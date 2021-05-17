using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class DialogueData
{
  [SerializeField]
  int id;
  public int Id { get {return id; } set { id = value;} }
  
  [SerializeField]
  string character;
  public string Character { get {return character; } set { character = value;} }
  
  [SerializeField]
  string text;
  public string Text { get {return text; } set { text = value;} }
  
  [SerializeField]
  string next;
  public string Next { get {return next; } set { next = value;} }
  
  [SerializeField]
  int jumpid;
  public int Jumpid { get {return jumpid; } set { jumpid = value;} }
  
  [SerializeField]
  int[] selects = new int[0];
  public int[] Selects { get {return selects; } set { selects = value;} }
  
  [SerializeField]
  string key;
  public string Key { get {return key; } set { key = value;} }
    
  [SerializeField]
  bool isShowLeft; 
  public bool IsShowLeft { get { return isShowLeft; } set { isShowLeft = value; } }

  [SerializeField]
  string musicEffect;
  public string MusicEffect { get { return musicEffect; } set { musicEffect = value; } }

  [SerializeField]
  string soundEffect;
  public string SoundEffect { get { return soundEffect; } set { soundEffect = value; } }

  [SerializeField]
  string vocalEffect;
  public string VocalEffect { get { return vocalEffect; } set { vocalEffect = value; } }
}