using UnityEngine;
using System;
using System.Collections.Generic;

public class FadeSequenceContainer : FContainer
{
	public List<NodeInfo> infos = new List<NodeInfo>();
	public int currentIndex = 0;
	public float transitionTime = 1.5f;

	public float timeUntilTransition;
	public float timeLeftInTransition;

	public FadeSequenceContainer()
	{

	}

	public void AddFadingNode(FNode node, float showDuration)
	{
		NodeInfo info = new NodeInfo();
		info.node = node;
		info.showDuration = showDuration;
		infos.Add(info);
	}

	public void Start()
	{
		AddChild(infos[0].node);
		timeUntilTransition = infos[0].showDuration;
		ListenForUpdate(HandleUpdate);
	}

	void HandleUpdate()
	{
		timeUntilTransition -= Time.deltaTime;
		if(timeUntilTransition < 0)
		{
			timeLeftInTransition = transitionTime + timeUntilTransition;
			float percent = Mathf.Clamp01((transitionTime-timeLeftInTransition)/transitionTime);

			NodeInfo current = infos[currentIndex];
			NodeInfo next = infos[(currentIndex+1)%infos.Count];

			if(next.node.container == null) AddChild(next.node);

			current.node.alpha = 1.0f - RXMath.GetSubPercent(percent,0.0f,0.55f);
			next.node.alpha = RXMath.GetSubPercent(percent,0.45f,1.0f);

			if(timeLeftInTransition < 0)
			{
				current.node.RemoveFromContainer();
				currentIndex = (currentIndex+1)%infos.Count;
				timeUntilTransition = infos[currentIndex].showDuration;
			}
		}
	}
	
	public class NodeInfo
	{
		public FNode node;
		public float showDuration;
	}
}