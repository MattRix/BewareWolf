using System;
public class MapGenerator {

public static MapData Generate(){

MapData md = new MapData();

md.Add(new FenceHMI(36,74));
md.Add(new FenceHMI(112,-65));
md.Add(new FenceHMI(-22,-98));
md.Add(new FenceHMI(-237,-33));
md.Add(new FenceHMI(-182,-93));
md.Add(new FenceVMI(-189,73));
md.Add(new FenceVMI(-48,-76));
md.Add(new FenceVMI(165,-14));
md.Add(new FenceVMI(84,-96));
md.Add(new FenceVMI(61,97));
md.Add(new FenceVMI(273,29));
md.Add(new House1MI(52,-49));
md.Add(new House1MI(132,32));
md.Add(new House1MI(-160,120));
md.Add(new House1MI(-183,-16));
md.Add(new StartPosMI(-62,-134));
md.Add(new StartPosMI(-257,127));
md.Add(new StartPosMI(-45,132));
md.Add(new StartPosMI(156,128));
md.Add(new StartPosMI(189,-115));
md.Add(new StartPosMI(237,27));
md.Add(new StartPosMI(-272,32));
md.Add(new StartPosMI(-228,-124));

return md;}}