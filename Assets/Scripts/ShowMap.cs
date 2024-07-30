using System.Linq;
using System.Xml;
using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// -=-=-=- //

public class ShowMap : MonoBehaviour
{
    public string level_name;
    int layer;
    GameObject actualObject;
    GameObject lastContent;
    GameObject lv;
    GameObject part;

    [MenuItem("Vectorier/Render object sequence")]
    public static void RenderMap()
    {

        // lists to store building and object filenames.
        List<string> buildings = new List<string>();
        List<string> objects = new List<string>();

        // Logs the level being rendered.
        Debug.Log("Rendering level " + GameObject.FindObjectOfType<ShowMap>().level_name);
        GameObject.FindObjectOfType<ShowMap>().lv = new GameObject(GameObject.FindObjectOfType<ShowMap>().level_name);  //makes a new game object with chosen level's name
        GameObject.FindObjectOfType<ShowMap>().lv.name = GameObject.FindObjectOfType<ShowMap>().level_name;

        XmlDocument level = new XmlDocument();
        level.Load(Application.dataPath + "/XML/" + GameObject.FindObjectOfType<ShowMap>().level_name);  //Loads the XML file corresponding to the level.

        // Iterates over sets and objects in the XML, extracting filenames.
        foreach (XmlNode node in level.DocumentElement.SelectSingleNode("/Root/Sets"))
        {
            buildings.Add(node.Attributes.GetNamedItem("FileName").Value); //adds which buildings xml is used into the list (ex. buildings.xml, buildings_downtown.xml)
            XmlDocument building = new XmlDocument();
            building.Load(Application.dataPath + "/XML/" + node.Attributes.GetNamedItem("FileName").Value); //sees which objects xml is used (ex. objects.xml, objects_downtown.xml)

            foreach (XmlNode b_node in building.DocumentElement.SelectSingleNode("/Root/Sets"))
                objects.Add(b_node.Attributes.GetNamedItem("FileName").Value); //adds all used object_xml into the list
        }

        // Iterates over track objects, checking if they have child objects
        foreach (XmlNode node in level.DocumentElement.SelectSingleNode("/Root/Track"))
        {
            if (node.Name == "Object")
            {
                if (node.HasChildNodes)
                {
                    if (node.FirstChild.FirstChild.Attributes["Name"] != null)
                    {
                        foreach (XmlNode content in node.FirstChild)
                        {
                            if (content.Name == "Object")
                            {
                                GameObject.FindObjectOfType<ShowMap>().layer += 1;
                                bool foundInBuildings = false;

                                foreach (string building_name in buildings)
                                {
                                    XmlDocument building = new XmlDocument();
                                    building.Load(Application.dataPath + "/XML/" + building_name); //loads the building xml file
                                    foreach (XmlNode b_node in building.DocumentElement.SelectSingleNode("/Root/Objects"))
                                    {

                                        //Check if the object has the correct name
                                        if (b_node.Name == "Object")
                                        {
                                            if (b_node.Attributes["Name"] != null)
                                            {

                                                //check if the object names in the building xml is used in level xml
                                                if (b_node.Attributes.GetNamedItem("Name").Value == content.Attributes.GetNamedItem("Name").Value)
                                                {
                                                    Debug.Log("Rendering part " + content.Attributes.GetNamedItem("Name").Value + " at X=" + content.Attributes.GetNamedItem("X").Value + " and Y=" + content.Attributes.GetNamedItem("Y").Value);
                                                    foundInBuildings = true; //found object in building xml
                                                    RenderSequence(content.Attributes.GetNamedItem("Name").Value, building_name, content.Attributes.GetNamedItem("X").Value, content.Attributes.GetNamedItem("Y").Value);
                                                }
                                            }
                                        }
                                    }
                                }

                                // If level didnt use the object from building xml
                                if (foundInBuildings == false)
                                {
                                    GameObject.FindObjectOfType<ShowMap>().layer += 1;
                                    Debug.Log("Rendering object " + content.Attributes.GetNamedItem("Name").Value + " at X=" + content.Attributes.GetNamedItem("X").Value + " and Y=" + content.Attributes.GetNamedItem("Y").Value);
                                    ConvertXmlToObject(content.Attributes.GetNamedItem("Name").Value, content.Attributes.GetNamedItem("X").Value, content.Attributes.GetNamedItem("Y").Value);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (XmlNode empty_object in node.FirstChild)
                {
                    foreach (XmlNode empty_object_content in empty_object.FirstChild)
                    {

                        if (empty_object_content.Name == "Object" && empty_object_content.Attributes["Name"] != null)
                        {
                            GameObject.FindObjectOfType<ShowMap>().layer += 1;
                            bool foundInBuildings = false;
                            foreach (string building_name in buildings)
                            {
                                XmlDocument building = new XmlDocument();
                                building.Load(Application.dataPath + "/XML/" + building_name);
                                foreach (XmlNode b_node in building.DocumentElement.SelectSingleNode("/Root/Objects"))
                                {

                                    // Check if the object has the correct name
                                    if (b_node.Name == "Object")
                                    {
                                        if (b_node.Attributes["Name"] != null)
                                        {
                                            if (b_node.Attributes.GetNamedItem("Name").Value == empty_object_content.Attributes.GetNamedItem("Name").Value)
                                            {
                                                Debug.Log("Rendering part " + empty_object_content.Attributes.GetNamedItem("Name").Value + " at X=" + empty_object_content.Attributes.GetNamedItem("X").Value + " and Y=" + empty_object_content.Attributes.GetNamedItem("Y").Value);
                                                foundInBuildings = true;
                                                RenderSequence(empty_object_content.Attributes.GetNamedItem("Name").Value, building_name, empty_object_content.Attributes.GetNamedItem("X").Value, empty_object_content.Attributes.GetNamedItem("Y").Value);
                                            }
                                        }
                                    }
                                }
                            }

                            if (foundInBuildings == false)
                            {
                                GameObject.FindObjectOfType<ShowMap>().layer += 1;
                                Debug.Log("Rendering object " + empty_object_content.Attributes.GetNamedItem("Name").Value + " at X=" + empty_object_content.Attributes.GetNamedItem("X").Value + " and Y=" + empty_object_content.Attributes.GetNamedItem("Y").Value);
                                ConvertXmlToObject(empty_object_content.Attributes.GetNamedItem("Name").Value, empty_object_content.Attributes.GetNamedItem("X").Value, empty_object_content.Attributes.GetNamedItem("Y").Value);
                            }
                        }
                    }
                }
            }
        }
    }

    static void RenderSequence(string seq_name, string building_name, string x, string y)
    {
        Debug.Log("Rendering...");
        XmlDocument building = new XmlDocument();
        building.Load(Application.dataPath + "/XML/" + building_name); //loads the building xml file

        foreach (XmlNode node in building.DocumentElement.SelectSingleNode("/Root/Objects"))
        {
            // Check if the object has the correct name
            if (node.Name == "Object")
            {
                if (node.Attributes.GetNamedItem("Name").Value == seq_name)
                {
                    GameObject.FindObjectOfType<ShowMap>().part = new GameObject(seq_name); //make a new game object
                    GameObject.FindObjectOfType<ShowMap>().part.name = seq_name; //set the name to sequence name
                    GameObject.FindObjectOfType<ShowMap>().part.transform.SetParent(GameObject.FindObjectOfType<ShowMap>().lv.transform);
                    GameObject.FindObjectOfType<ShowMap>().part.transform.localPosition = new Vector3(
                        float.Parse(x) / 100,
                        -float.Parse(y) / 100,
                        0
                    );

                    // Search for each node in the object 
                    foreach (XmlNode content in node.FirstChild)
                    {
                        if (content.Name == "Object")
                        {
                            GameObject.FindObjectOfType<ShowMap>().layer += 1;

                            if (content.Attributes["Name"] != null)
                            {
                                Debug.Log("Found object with name " + content.Attributes.GetNamedItem("Name").Value + " and coordinates " + content.Attributes.GetNamedItem("X").Value + " " + content.Attributes.GetNamedItem("Y").Value);
                                ConvertXmlToObject(content.Attributes.GetNamedItem("Name").Value, content.Attributes.GetNamedItem("X").Value, content.Attributes.GetNamedItem("Y").Value);
                            }
                            else
                            {
                                Debug.Log("Found object with coordinates " + content.Attributes.GetNamedItem("X").Value + " " + content.Attributes.GetNamedItem("Y").Value);

                                foreach (XmlNode child in content.FirstChild)
                                {
                                    if (child.Name == "Image")
                                    {
                                        GameObject.FindObjectOfType<ShowMap>().InstantiateObject(child, "Unnamed-object", content.Attributes.GetNamedItem("X").Value, content.Attributes.GetNamedItem("Y").Value);
                                    }
                                    else if (child.Name == "Trigger")
                                    {
                                        GameObject.FindObjectOfType<ShowMap>().InstantiateObject(child, "Unnamed-object", content.Attributes.GetNamedItem("X").Value, content.Attributes.GetNamedItem("Y").Value);
                                    }
                                    else if (child.Name == "Area")
                                    {
                                        GameObject.FindObjectOfType<ShowMap>().InstantiateObject(child, "Unnamed-object", content.Attributes.GetNamedItem("X").Value, content.Attributes.GetNamedItem("Y").Value);
                                    }
                                    else if (child.Name == "Object")
                                    {
                                        GameObject.FindObjectOfType<ShowMap>().InstantiateObject(child, "Unnamed-object", content.Attributes.GetNamedItem("X").Value, content.Attributes.GetNamedItem("Y").Value);
                                    }
                                    else if (child.Name == "Platform")
                                    {
                                        GameObject.FindObjectOfType<ShowMap>().InstantiateObject(child, "Unnamed-object", content.Attributes.GetNamedItem("X").Value, content.Attributes.GetNamedItem("Y").Value);
                                    }
                                }
                            }
                        }
                        else if (content.Name == "Image")
                        {
                            GameObject.FindObjectOfType<ShowMap>().layer += 1;
                            Debug.Log("Found image with name " + content.Attributes.GetNamedItem("ClassName").Value + " and coordinates " + content.Attributes.GetNamedItem("X").Value + " " + content.Attributes.GetNamedItem("Y").Value);
                            GameObject.FindObjectOfType<ShowMap>().actualObject = new GameObject(content.Attributes.GetNamedItem("ClassName").Value);
                            GameObject.FindObjectOfType<ShowMap>().actualObject.name = content.Attributes.GetNamedItem("ClassName").Value;
                            GameObject.FindObjectOfType<ShowMap>().actualObject.transform.SetParent(GameObject.FindObjectOfType<ShowMap>().part.transform);
                            GameObject.FindObjectOfType<ShowMap>().actualObject.transform.localPosition = new Vector3(0, 0, 0);
                            GameObject.FindObjectOfType<ShowMap>().InstantiateObject(content, content.Attributes.GetNamedItem("ClassName").Value, content.Attributes.GetNamedItem("X").Value, content.Attributes.GetNamedItem("Y").Value);
                            GameObject.FindObjectOfType<ShowMap>().actualObject = null;
                        }
                        else if (content.Name == "Trigger" || content.Name == "Area")
                        {
                            GameObject.FindObjectOfType<ShowMap>().layer += 1;
                            if (content.Name == "Trigger")
                                Debug.Log("Found trigger with name " + content.Attributes.GetNamedItem("Name").Value + " and coordinates " + content.Attributes.GetNamedItem("X").Value + " " + content.Attributes.GetNamedItem("Y").Value);
                            else
                                Debug.Log("Found area with name " + content.Attributes.GetNamedItem("Name").Value + " and coordinates " + content.Attributes.GetNamedItem("X").Value + " " + content.Attributes.GetNamedItem("Y").Value);

                            GameObject.FindObjectOfType<ShowMap>().actualObject = new GameObject(content.Attributes.GetNamedItem("Name").Value);
                            GameObject.FindObjectOfType<ShowMap>().actualObject.name = content.Attributes.GetNamedItem("Name").Value;
                            GameObject.FindObjectOfType<ShowMap>().actualObject.transform.SetParent(GameObject.FindObjectOfType<ShowMap>().part.transform);
                            GameObject.FindObjectOfType<ShowMap>().actualObject.transform.localPosition = new Vector3(0, 0, 0);
                            GameObject.FindObjectOfType<ShowMap>().InstantiateObject(content, content.Attributes.GetNamedItem("Name").Value, content.Attributes.GetNamedItem("X").Value, content.Attributes.GetNamedItem("Y").Value);
                            GameObject.FindObjectOfType<ShowMap>().actualObject = null;
                        }
                    }
                }
            }
        }

        GameObject.FindObjectOfType<ShowMap>().part = null;
    }

    static void ConvertXmlToObject(string object_name, string x, string y)
    {
        Debug.Log("Converting...");

        // Load all object XMLs
        XmlDocument obj = new XmlDocument();
        obj.Load(Application.dataPath + "/XML/objects.xml");
        bool objectFound = false;
        int doc_num = 0;

        // Search for the selected object in the object XMLs
        while (objectFound == false & doc_num < 3)
        {
            if (doc_num == 0)
            {
                obj.Load(Application.dataPath + "/XML/objects.xml");
            }
            else if (doc_num == 1)
            {
                obj.Load(Application.dataPath + "/XML/objects_downtown.xml");
            }
            else if (doc_num == 2)
            {
                obj.Load(Application.dataPath + "/XML/objects_construction.xml");
            }

            foreach (XmlNode node in obj.DocumentElement.SelectSingleNode("/Root/Objects"))
            {
                // Check if the object has the correct name
                if (node.Name == "Object")
                    if (node.Attributes.GetNamedItem("Name").Value == object_name)
                    {
                        objectFound = true;

                        // Search for each node in the object 
                        foreach (XmlNode content in node.FirstChild)
                        {
                            if (content.Name == "Image")
                                GameObject.FindObjectOfType<ShowMap>().InstantiateObject(content, object_name, x, y);
                            else if (content.Name == "Trigger")
                                GameObject.FindObjectOfType<ShowMap>().InstantiateObject(content, object_name, x, y);
                            else if (content.Name == "Area")
                                GameObject.FindObjectOfType<ShowMap>().InstantiateObject(content, object_name, x, y);
                            else if (content.Name == "Object")
                                GameObject.FindObjectOfType<ShowMap>().InstantiateObject(content, object_name, x, y);
                            else if (content.Name == "Platform")
                                GameObject.FindObjectOfType<ShowMap>().InstantiateObject(content, object_name, x, y);
                        }
                    }
            }
            doc_num += 1;
        }

        GameObject.FindObjectOfType<ShowMap>().actualObject = null;
        Debug.Log("Conversion done!");
    }

    void InstantiateObject(XmlNode content, string object_name, string x, string y)
    {
        // Debug all content found in the object
        if (content.Name == "Image")
        {
            Debug.Log("Found Image : " + content.Attributes.GetNamedItem("ClassName").Value);
        }
        else if (content.Name == "Trigger")
        {
            if (content.Attributes["Name"] != null)
            {
                Debug.Log("Found Trigger : " + content.Attributes.GetNamedItem("Name").Value);
            }
            else
            {
                Debug.Log("Found Trigger : " + "Trigger-" + object_name);
            }
        }
        else if (content.Name == "Area")
        {
            Debug.Log("Found Trick : " + content.Attributes.GetNamedItem("Name").Value);
        }
        else if (content.Name == "Object")
        {
            Debug.Log("Found Object : " + content.Attributes.GetNamedItem("Name").Value);
        }

        // Place the image using every information the xml provide (X, Y, Width, Height, ClassName)
        if (content.Name == "Object" && !content.Attributes.GetNamedItem("Name").Value.Contains("Trigger"))
        {
            ConvertXmlToObject(
                content.Attributes.GetNamedItem("Name").Value,
                content.Attributes.GetNamedItem("X").Value,
                content.Attributes.GetNamedItem("Y").Value
            );
        }
        else
        {
            if (actualObject == null)
            {
                // Create a new GameObject with the selected object
                actualObject = new GameObject(object_name);

                if (part != null)
                {
                    actualObject.transform.SetParent(part.transform);
                }
                else
                {
                    actualObject.transform.SetParent(lv.transform);
                    actualObject.transform.localPosition = new Vector3(
                    float.Parse(x) / 100,
                    -float.Parse(y) / 100,
                    0
                );
                }

                // Name it correctly
                actualObject.name = object_name;
            }

            // vv  If the content is an image  vv
            if (content.Name == "Image")
            {
                // Usage of ClassName value (To name the new object)
                lastContent = new GameObject(content.Attributes.GetNamedItem("ClassName").Value);

                // Reusage of ClassName value (To place the texture)
                lastContent.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/" + content.Attributes.GetNamedItem("ClassName").Value);

                // Check if the image is rotated (by checking if there is a Matrix node)
                if (content.HasChildNodes)
                {
                    // Get into the Matrix node
                    foreach (XmlNode matrixNode in content.LastChild.FirstChild)
                    {
                        if (matrixNode.Name == "Matrix" && matrixNode.Attributes.GetNamedItem("A").Value != content.Attributes.GetNamedItem("Width").Value)
                        {
                            if (matrixNode.Attributes.GetNamedItem("A").Value != "")
                            {
                                lastContent.transform.rotation = Quaternion.Euler(
                                    0,
                                    float.Parse(content.Attributes.GetNamedItem("Width").Value, CultureInfo.InvariantCulture) / float.Parse(matrixNode.Attributes.GetNamedItem("A").Value, CultureInfo.InvariantCulture) * 180f,
                                    0
                                );
                            }
                        }
                    }
                }
            }

            // vv  If the content is a trigger  vv
            else if (content.Name == "Trigger")
            {
                if (content.Attributes["Name"] != null)
                {
                    // Usage of Name value (To name the new object)
                    lastContent = new GameObject(content.Attributes.GetNamedItem("Name").Value);
                }
                else
                {
                    // Usage of Statistic value (To name the new object)
                    lastContent = new GameObject("Trigger-" + object_name);
                }
                // Reusage of ClassName value (To place the texture)
                lastContent.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/trigger");
            }

            // vv  If the content is a Trick  vv
            else if (content.Name == "Area" || content.Name == "Object" && content.OuterXml.Contains("Trigger"))
            {
                //Usage of Name value (To name the new object)
                lastContent = new GameObject(content.Attributes.GetNamedItem("Name").Value);

                if (content.Attributes.GetNamedItem("Name").Value.Contains("Trigger") && content.Attributes["ItemName"] != null)
                {
                    // Reusage of ClassName value (To place the texture)
                    lastContent.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/tricks/TRACK_" + content.Attributes.GetNamedItem("ItemName").Value);
                }
                else
                {
                    // Reusage of ClassName value (To place the texture)
                    lastContent.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/trigger");
                }
            }

            // vv  If the content is a hitbox  vv
            else if (content.Name == "Platform")
            {
                lastContent = new GameObject("Platform-" + object_name);
                lastContent.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/trick");
            }

            // vv  Universal action  vv
            // Place the new image into the selected object
            lastContent.GetComponent<SpriteRenderer>().transform.SetParent(actualObject.transform);

            if (content.Name != "Object")
            {
                lastContent.transform.localPosition = new Vector3(
                    float.Parse(content.Attributes.GetNamedItem("X").Value) / 100 + Math.Abs(lastContent.transform.rotation.y) * float.Parse(content.Attributes.GetNamedItem("Width").Value) / 100,
                    -float.Parse(content.Attributes.GetNamedItem("Y").Value) / 100,
                    0
                );
            }
            else
            {
                lastContent.transform.localPosition = new Vector3(
                    float.Parse(content.Attributes.GetNamedItem("X").Value) / 100,
                    -float.Parse(content.Attributes.GetNamedItem("Y").Value) / 100,
                    0
                );
            }

            if (lastContent.GetComponent<SpriteRenderer>().sprite.name.Contains("TRICK"))
            {
                lastContent.transform.localScale = new Vector3(1, 1, 0);
            }
            else if (content.Attributes["Width"] != null)
            {
                // Usage of Width and Height value
                lastContent.transform.localScale = new Vector3(
                    float.Parse(content.Attributes.GetNamedItem("Width").Value) / lastContent.GetComponent<SpriteRenderer>().sprite.texture.width,
                    float.Parse(content.Attributes.GetNamedItem("Height").Value) / lastContent.GetComponent<SpriteRenderer>().sprite.texture.height,
                    0
                );
            }

            // VERY IMPORTANT: Every GameObject with the tag "Object" will be counted in the final build, else ignored
            actualObject.tag = "Object";
            lastContent.GetComponent<SpriteRenderer>().sortingOrder = layer;
        }
    }
}