using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Globalization;

public class LoadSaveSystem : MonoBehaviour {

       public static void SaveFile(string file_name)
    {
      
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + file_name + ".dat");

        PatrolData data = ScriptableObject.CreateInstance<PatrolData>();

        data.static_patrol = true;

        bf.Serialize(file, data);
        file.Close();
    }

    public static void LoadFile(string file_name)
    {
        if(File.Exists(Application.persistentDataPath + "/" + file_name + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + file_name + ".dat", FileMode.Open);     
            PatrolData p = (PatrolData)bf.Deserialize(file);
            Debug.Log(p.static_patrol);

            file.Close();
        }        
    }

    public static void SaveXML(string file_name)
    {
        float test = 50;

        Debug.Log(Application.persistentDataPath);

        // Stream the file with a File Stream. (Note that File.Create() 'Creates' or 'Overwrites' a file.)
        FileStream file = File.Create(Application.persistentDataPath + "/" + file_name + ".xml");
        // Create a new Player_Data.
        PatrolData data = ScriptableObject.CreateInstance<PatrolData>();
        //Save the data.
        data.is_synchronized = true;
        data.path_attached = GameObject.Find("Neutral_trigger");
        data.give_permission_pos = 15;
        data.path[0] = Vector3.forward;

        //Serialize to xml
        DataContractSerializer bf = new DataContractSerializer(data.GetType());
        MemoryStream streamer = new MemoryStream();

        //Serialize the file
        bf.WriteObject(streamer, data);
        streamer.Seek(0, SeekOrigin.Begin);

        //Save to disk
        file.Write(streamer.GetBuffer(), 0, streamer.GetBuffer().Length);

        // Close the file to prevent any corruptions
        file.Close();

        //string result = XElement.Parse(Encoding.ASCII.GetString(streamer.GetBuffer()).Replace("\0", "")).ToString();
        string result = XElement.Parse(Encoding.ASCII.GetString(streamer.GetBuffer())).ToString();

        Debug.Log("Serialized Result: " + result);
    }

    public static void LoadXML(string file_name)
    {
        float test = 50;

        Debug.Log(Application.persistentDataPath);

        // Stream the file with a File Stream. (Note that File.Create() 'Creates' or 'Overwrites' a file.)
        //FileStream file = File.Open(Application.persistentDataPath + "/" + file_name + ".dat", FileMode.Open);
        // Create a new Player_Data.
        PatrolData data = new PatrolData(5);

        //Serialize to xml
        DataContractSerializer bf = new DataContractSerializer(data.GetType());
        MemoryStream streamer = new MemoryStream();

        //file_name = Path.Combine(this.path, Application.persistentDataPath, file_name + ".dat");

        //DataContractSerializer dcs = new DataContractSerializer(typeof(Games));
        FileStream fs = new FileStream(Application.persistentDataPath + "/" + file_name + ".xml", FileMode.Open);
        XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

        data = (PatrolData)bf.ReadObject(reader);
        reader.Close();
        fs.Close();


        ////Serialize the file
        //data = (PatrolData)bf.ReadObject(streamer);

        //Debug.Log(data.is_synchronized);
        //Debug.Log(data.synchronized_Rhandor);
        //Debug.Log(data.give_permission_pos);
        //Debug.Log(data.path[0]);

        //bf.WriteObject(streamer, data);
        //streamer.Seek(0, SeekOrigin.Begin);

        //Save to disk
        //file.Write(streamer.GetBuffer(), 0, streamer.GetBuffer().Length);

        // Close the file to prevent any corruptions
        //file.Close();
       
        //string result = XElement.Parse(Encoding.ASCII.GetString(streamer.GetBuffer()).Replace("\0", "")).ToString();
        //Debug.Log("Serialized Result: " + result);

    }
}

[CreateAssetMenu]
public class PatrolData : ScriptableObject
{
    public PATROL_TYPE type;                    // Type of the patrol: NEUTRAL or ALERT
    public bool expanded;                       // For Toggle Editor use, useful to expand the different patrol info

    public bool static_patrol;                  // Rhandor will remain on its initial position without doing patrols
    public bool loop;                           // Rhandor will change its direction upon reaching last position and the same 
                                                // to the first position.If false, Rhandor will close its patrol path.

    public bool is_synchronized;                // Declares a synchronized patrol with ONLY other Rhandor (so far)
    
    public GameObject synchronized_Rhandor;     // The Rhandor synchronized with
    public int give_permission_pos;             // Position where Rhandor will give permission to move to the synchronized Rhandor.
    public int ask_for_permission_pos;          // Position where Rhandor will ask for permission to its synchronized Rhandor.
    public bool can_give_permission = false;    // Rhandor will give permissions of movement.
    public bool can_get_permission = false;     // Rhandor will recieve permissions to resume its movement.

    public GameObject path_attached;            // The GameObject that contains waypoints as childs and represents the patrol path.

    public int size;                            // Number of waypoints
    public Vector3[] path;                      // The colection of positions that conforms the patrol
    public float[] stop_times;                  // Number of seconds that the enemy will stop at the selected position
    public bool[] trigger_movement;             // This position will trigger the movement of the synchronized Rhandor
    public bool[] recieve_trigger;              // Response to a trigger movement from the synchronzied Rhandor

    public PatrolData(int _size)
    {
        _size = 1;

        path = new Vector3[_size];
        stop_times = new float[_size];
        trigger_movement = new bool[_size];
        recieve_trigger = new bool[_size];
    }
}