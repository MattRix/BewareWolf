using System;
using UnityEngine;

public class TOFonts
{
	public static string TINY = "DiscoTiny";
	
	public static string MEDIUM = "TempestaSevenCondensed";
	//	public static string MEDIUM_NARROW = "TempestaSevenCompressed";
	public static string MEDIUM_BOLD = "TempestaSevenCondensedBold";
	
	public static string SMALL = "TempestaFiveCondensed";
	public static string SMALL_NARROW = "TempestaFiveCompressed";
	public static string SMALL_BOLD = "TempestaFiveCondensedBold";
	
	public static string COIN_CHAR = ((char)172).ToString(); //¬ 172
	public static string BUX_CHAR = ((char)186).ToString(); //º 186
	public static string MULTIPLY_X_CHAR = ((char)215).ToString(); //× 215
	public static string SPACE_CHAR = ((char)160).ToString(); //space char!
	public static string SPACE_COIN = SPACE_CHAR+COIN_CHAR;
	public static string SPACE_BUX = SPACE_CHAR+BUX_CHAR;
	
	public static void Load()
	{
		LoadFont(TINY,0,0,0,0);
		LoadFont(SMALL,0,0,0,0);
		LoadFont(SMALL_BOLD,0,0,0,0);
		LoadFont(SMALL_NARROW,0,0,0,0);
		LoadFont(MEDIUM,0,0,0,0); 
		LoadFont(MEDIUM_BOLD,0,0,1,0); 
	}
	
	private static void LoadFont(string fontName, float offsetX, float offsetY,float lineHeightOffset, float kerningOffset)
	{
		//offsetY -= 1f; //they're all off by 1
		offsetY -= 2f;
		lineHeightOffset -= 6;

		FTextParams textParams;
		
		textParams = new FTextParams();
		textParams.kerningOffset = kerningOffset;
		textParams.lineHeightOffset = lineHeightOffset;

		Futile.atlasManager.LoadFont(fontName,"Fonts/"+fontName, "Fonts/"+fontName, offsetX,offsetY,textParams);
	}
}

