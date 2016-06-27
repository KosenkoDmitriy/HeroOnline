using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ObscuredPrefsTest : MonoBehaviour
{
	private const string PREFS_NAME = "name";
	private const string PREFS_MONEY = "money";
	private const string PREFS_LEVEL = "level";
	private const string PREFS_LIFE_BAR = "lifeBar";
	private const string PREFS_GAME_COMPLETE = "gameComplete";
	private const string PREFS_LONG = "demoLong";
	private const string PREFS_DOUBLE = "demoDouble";
	private const string PREFS_BYTE_ARRAY = "demoByteArray";
	private const string PREFS_VECTOR3 = "demoVector3";

	// This is a small trick - it allows to hide your encryption key 
	// in the serialized MonoBehaviour in the Editor inpector, 
	// outside of the IL bytecode, so to recover it hacker needs to 
	// know how to reach it in the Unity scene ;)
	public string encryptionKey = "change me!";

	internal string gameData = "";

	private void OnApplicationQuit()
	{
		PlayerPrefs.DeleteKey(PREFS_NAME);
		PlayerPrefs.DeleteKey(PREFS_MONEY);
		PlayerPrefs.DeleteKey(PREFS_LEVEL);
		PlayerPrefs.DeleteKey(PREFS_LIFE_BAR);

		ObscuredPrefs.DeleteKey(PREFS_NAME);
		ObscuredPrefs.DeleteKey(PREFS_MONEY);
		ObscuredPrefs.DeleteKey(PREFS_LEVEL);
		ObscuredPrefs.DeleteKey(PREFS_LIFE_BAR);
		ObscuredPrefs.DeleteKey(PREFS_GAME_COMPLETE);
		ObscuredPrefs.DeleteKey(PREFS_LONG);
		ObscuredPrefs.DeleteKey(PREFS_DOUBLE);
		ObscuredPrefs.DeleteKey(PREFS_BYTE_ARRAY);
		ObscuredPrefs.DeleteKey(PREFS_VECTOR3);
	}

	private void Awake()
	{
		ObscuredPrefs.SetNewCryptoKey(encryptionKey);
	}

	public void SaveGame(bool obscured)
	{
		if (obscured)
		{
			ObscuredPrefs.SetString(PREFS_NAME, "obscured focus oO");
			ObscuredPrefs.SetInt(PREFS_MONEY, 1500);
			ObscuredPrefs.SetInt(PREFS_LEVEL, 2);
			ObscuredPrefs.SetFloat(PREFS_LIFE_BAR, 25.9f);
			ObscuredPrefs.SetBool(PREFS_GAME_COMPLETE, true);
			ObscuredPrefs.SetLong(PREFS_LONG, 3457657543456775432L);
			ObscuredPrefs.SetDouble(PREFS_DOUBLE, 345765.1312315678d);
			ObscuredPrefs.SetByteArray(PREFS_BYTE_ARRAY, new byte[]{44, 104, 43, 32});
			ObscuredPrefs.SetVector3(PREFS_VECTOR3, new Vector3(123.312f, 453.12345f,1223f));
			
			Debug.Log("Game saved using ObscuredPrefs. Try to find and change saved data now! ;)");
		}
		else
		{
			PlayerPrefs.SetString(PREFS_NAME, "focus :D");
			PlayerPrefs.SetInt(PREFS_MONEY, 2100);
			PlayerPrefs.SetInt(PREFS_LEVEL, 4);
			PlayerPrefs.SetFloat(PREFS_LIFE_BAR, 88.4f);
			Debug.Log("Game saved with regular PlayerPrefs. Try to find and change saved data now (it's easy)!");
		}
		ObscuredPrefs.Save();
	}

	public void ReadSavedGame(bool obscured)
	{
		if (obscured)
		{
			gameData = "Name: " + ObscuredPrefs.GetString(PREFS_NAME) + "\n";
			gameData += "Money: " + ObscuredPrefs.GetInt(PREFS_MONEY) + "\n";
			gameData += "Level: " + ObscuredPrefs.GetInt(PREFS_LEVEL) + "\n";
			gameData += "Life bar: " + ObscuredPrefs.GetFloat(PREFS_LIFE_BAR) + "\n";
			gameData += "bool: " + ObscuredPrefs.GetBool(PREFS_GAME_COMPLETE) + "\n";
			gameData += "long: " + ObscuredPrefs.GetLong(PREFS_LONG) + "\n";
			gameData += "double: " + ObscuredPrefs.GetDouble(PREFS_DOUBLE) + "\n";
			byte[] ba = ObscuredPrefs.GetByteArray(PREFS_BYTE_ARRAY, 0, 4);
			gameData += "Vector3: " + ObscuredPrefs.GetVector3(PREFS_VECTOR3) + "\n";
			gameData += "byte[]: {" + ba[0] + "," + ba[1] + "," + ba[2] + "," + ba[3] + "}";
		}
		else
		{
			gameData = "Name: " + PlayerPrefs.GetString(PREFS_NAME) + "\n";
			gameData += "Money: " + PlayerPrefs.GetInt(PREFS_MONEY) + "\n";
			gameData += "Level: " + PlayerPrefs.GetInt(PREFS_LEVEL) + "\n";
			gameData += "Life bar: " + PlayerPrefs.GetFloat(PREFS_LIFE_BAR);
		}
	}
}