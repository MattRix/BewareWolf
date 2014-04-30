using UnityEngine;
using System;
using System.Collections.Generic;

public class DenomMaker
{
	public static List<int> GetDenoms(int[] denoms, int totalValue, int numCoins)
	{
		List<int> resultCoins = new List<int>(numCoins+10);

		if(totalValue <= 0) return resultCoins;

		numCoins = Math.Max(1,Math.Min(totalValue,numCoins)); //can't make more coins than we have value for!

		int valueLeftToAdd = totalValue;
		int numCoinsLeft = numCoins;

		int denomCount = denoms.Length;

		while(numCoinsLeft > 0 && valueLeftToAdd > 0)
		{
			int avg = valueLeftToAdd/numCoinsLeft;

			int pickedCoin = -1;

			for(int d = 0; d<denomCount; d++)
			{
				if(denoms[d] > avg)
				{
					pickedCoin = denoms[d-1];
					break;
				}
			}

			if(pickedCoin == -1)
			{
				pickedCoin = denoms[denomCount-1];
			}

			resultCoins.Add(pickedCoin);
			valueLeftToAdd -= pickedCoin;
			numCoinsLeft--;

			if(numCoinsLeft == 0 && valueLeftToAdd > 0)
			{
				numCoinsLeft++;
			}
		}

		return resultCoins;
	}
}


