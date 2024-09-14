using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;


// -=-=-=- //

public class BuildMapVec2 : MonoBehaviour
{

    // ReSharper disable once InconsistentNaming
    internal string vectorFilePath { get; set; }

    void Awake()
    {
        vectorFilePath = VectorierSettings.RoomsDirectory;
    }

    public static event Action MapBuilt;

    // Flag to indicate if the build is for running the game
    public static bool IsBuildForRunGame { get; set; } = false;



    // -=-=-=- //


    // Level Settings
    [Header("Level Settings")]

    [Tooltip("Level that will get overridden.")]
    public string mapToOverride = "escape_room";

    // Miscellaneous
    [Header("Miscellaneous")]
    public bool debugObjectWriting;

    // -=-=-=- //

    [MenuItem("Vectorier/BuildMap (Vec2)")]
    public static void BuildXml() { Build(false, false, true); }


    // -=-=-=- //


    public static void Build(bool useDZ, bool compileMap, bool moveXml)
    {
        // This is used to cache the BuildMap component. This is done to avoid the FindObjectOfType method in loop and other places.
        // This is a slow operation.
        var buildMap = FindObjectOfType<BuildMapVec2>();

#if UNITY_EDITOR
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
#endif

        if (string.IsNullOrEmpty(buildMap.vectorFilePath))
        {
            buildMap.vectorFilePath = VectorierSettings.RoomsDirectory;
        }
        Debug.Log("Building...");

        // Start the stopwatch
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // -=-=-=- //

        //Erase last build
        File.Delete(Application.dataPath + "/XML/build-map - vec2.xml");
        File.Copy(Application.dataPath + "/XML/empty-map-DONT-MODIFY-vec2.xml", Application.dataPath + "/XML/build-map - vec2.xml");

        //Open the object.xml
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/XML/build-map - vec2.xml");

        //Search for the selected object in the object.xml
        foreach (XmlNode node in xml.DocumentElement.SelectSingleNode("/Root/Track"))
        {


            //Set the properties into the level
            buildMap.SetLevelProperties(xml, node);

            // Get all GameObjects with tag "Image", then arrange them based on sorting order
            GameObject[] imagesInScene = GameObject.FindGameObjectsWithTag("Image")
                                        .OrderBy(obj => obj.GetComponent<SpriteRenderer>().sortingOrder)
                                        .ToArray();

            //Write every GameObject with tag "Object", "Image", "Platform", "Area" and "Trigger" in the build-map.xml

            // Image
            foreach (GameObject imageInScene in imagesInScene)
            {
                UnityEngine.Transform parent = imageInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }

                buildMap.ConvertToImage(node, xml, imageInScene);
            }


            // Object
            foreach (GameObject objectInScene in GameObject.FindGameObjectsWithTag("Object"))
            {
                UnityEngine.Transform parent = objectInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }

                buildMap.ConvertToObject(node, xml, objectInScene);
            }

            // Animation
            foreach (GameObject animationInScene in GameObject.FindGameObjectsWithTag("Animation"))
            {
                UnityEngine.Transform parent = animationInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                buildMap.ConvertToAnimation(node, xml, animationInScene);
            }

            // Platforms
            foreach (GameObject platformInScene in GameObject.FindGameObjectsWithTag("Platform"))
            {
                UnityEngine.Transform parent = platformInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                buildMap.ConvertToPlatform(node, xml, platformInScene);
            }

            // Trapezoid
            foreach (GameObject trapezoidInScene in GameObject.FindGameObjectsWithTag("Trapezoid"))
            {
                UnityEngine.Transform parent = trapezoidInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                buildMap.ConvertToTrapezoid(node, xml, trapezoidInScene);
            }

            // Trigger
            foreach (GameObject triggerInScene in GameObject.FindGameObjectsWithTag("Trigger"))
            {
                UnityEngine.Transform parent = triggerInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                buildMap.ConvertToTrigger(node, xml, triggerInScene);
            }

            foreach (GameObject triggerInScene in GameObject.FindGameObjectsWithTag("Dynamic Trigger"))
            {
                UnityEngine.Transform parent = triggerInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                buildMap.ConvertToDynamicTrigger(node, xml, triggerInScene);
            }

            // Area
            foreach (GameObject areaInScene in GameObject.FindGameObjectsWithTag("Area"))
            {
                UnityEngine.Transform parent = areaInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                buildMap.ConvertToArea(node, xml, areaInScene);
            }


            // In
            foreach (GameObject InTagInScene in GameObject.FindGameObjectsWithTag("In"))
            {
                UnityEngine.Transform parent = InTagInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                buildMap.ConvertToIn(node, xml, InTagInScene);
            }

            // Out
            foreach (GameObject OutTagInScene in GameObject.FindGameObjectsWithTag("Out"))
            {
                UnityEngine.Transform parent = OutTagInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                buildMap.ConvertToOut(node, xml, OutTagInScene);
            }

            // Object Reference
            foreach (GameObject ObjectReferenceTagInScene in GameObject.FindGameObjectsWithTag("Object Reference"))
            {
                UnityEngine.Transform parent = ObjectReferenceTagInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                buildMap.ConvertToObjectReference(node, xml, ObjectReferenceTagInScene);
            }

            // Camera
            foreach (GameObject camInScene in GameObject.FindGameObjectsWithTag("Camera"))
            {
                //Note: This is actually a trigger, but with camera zoom properties
                buildMap.ConvertToCamera(node, xml, camInScene);
            }

            // Dynamic
            foreach (GameObject dynamicInScene in GameObject.FindGameObjectsWithTag("Dynamic"))
            {
                UnityEngine.Transform dynamicInSceneTransform = dynamicInScene.transform;
                buildMap.ConvertToDynamic(node, xml, dynamicInScene, dynamicInSceneTransform);
            }



        }

        // vv  Build level directly into Vector (sweet !)  vv
        if (compileMap)
        {
            buildMap.StartDzip(useDZ);
        }

        if (moveXml)
        {
            buildMap.MoveXML(useDZ);
        }


        // Show Stopwatch
        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;
        string formattedTime = ts.TotalSeconds.ToString("F3", CultureInfo.InvariantCulture);

        Debug.Log($"Building done! ({formattedTime} seconds)");

        // -=-=-=- //


        // If the build was for running the game, invoke the MapBuilt event
        if (IsBuildForRunGame)
        {
            MapBuilt?.Invoke();

            // Reset the flag after the build
            IsBuildForRunGame = false;
        }
    }

    void StartDzip(bool useDZ)
    {
        // Check if Vector.exe is running - if yes, close it
        Process[] processes = Process.GetProcessesByName("Vector");
        foreach (Process process in processes)
        {
            if (!process.HasExited)
            {
                Debug.LogWarning("Closing Vector (be careful next time)");

                process.Kill();
                process.WaitForExit();
            }
        }

        // Start compressing levels into level_xml.dz
        string batchFileName = useDZ ? "compile-map.bat" : "compile-map-optimized.bat";
        string batchFilePath = Path.Combine(Application.dataPath, "XML/dzip", batchFileName);
        string batchDirectory = Path.GetDirectoryName(batchFilePath);

        if (!File.Exists(batchFilePath))
        {
            Debug.LogError($"Batch file not found: {batchFilePath}");
            return;
        }

        Process batchProcess = new Process
        {
            StartInfo = {
        FileName = batchFilePath,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true,
        WorkingDirectory = batchDirectory // Set working directory
    }
        };

        // Start the process
        try
        {
            batchProcess.Start();

            // Wait for the process to exit
            batchProcess.WaitForExit();

            // Check exit code if necessary
            if (batchProcess.ExitCode != 0)
            {
                string errorOutput = batchProcess.StandardError.ReadToEnd();
                Debug.LogError($"dzip.exe encountered an error: {errorOutput}");
            }
            else
            {
                // Move the file if the process succeeded
                string sourceFilePath = Path.Combine(Application.dataPath, "XML/dzip/level_xml.dz");
                string destinationFilePath = Path.Combine(vectorFilePath, "level_xml.dz");

                if (File.Exists(sourceFilePath))
                {
                    if (File.Exists(destinationFilePath))
                    {
                        File.Delete(destinationFilePath);
                    }

                    File.Copy(sourceFilePath, destinationFilePath);
                }
                else
                {
                    Debug.LogError("level_xml.dz was not found! Check if your Vector path is correct");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start dzip.exe: {e.Message}");
        }
        finally
        {
            // Ensure to close the process resources
            batchProcess.Close();
        }

        // Trigger the event if the build was intended for running the game
        if (IsBuildForRunGame)
        {
            MapBuilt?.Invoke();

            // Reset flag after building
            IsBuildForRunGame = false;
        }
    }

    void MoveXML(bool useDZ)
    {
        // Start the process
        {
            // Move the file if the process succeeded
            string sourceFilePath = Path.Combine(Application.dataPath, "XML/dzip/level_xml/" + mapToOverride + ".xml");
            string destinationFilePath = Path.Combine(vectorFilePath, mapToOverride + ".xml");

            if (!File.Exists(destinationFilePath))
            {
                Debug.LogError("file doesnt exist lol");
                return;
            }
            if (File.Exists(sourceFilePath))
            {
                if (File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }

                File.Copy(sourceFilePath, destinationFilePath);
            }
            else
            {
                Debug.LogError("the xml file was not found smh (how do u even get this)");
            }
        }
    }



    // -=-=-=-=-=- //

    void SetLevelProperties(XmlDocument xml, XmlNode objectNode)
    {
        // Find all object
        GameObject[] allObj = FindObjectsOfType<GameObject>();
        XmlNode rootNode = xml.DocumentElement.SelectSingleNode("/Root");

        // Set the background
        XmlNode objNode = xml.SelectSingleNode("/Root/Track");


        // Set the music


        // Set player, hunter properties
        foreach (GameObject allObjects in allObj) //loop to see if the object has buildmap component under it
        {
            BuildMapVec2 buildMap = allObjects.GetComponent<BuildMapVec2>();
        }
    }


    void ConvertToImage(XmlNode node, XmlDocument xml, GameObject imageInScene)
    {
        //Debug in log every images it write
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(imageInScene.name, @" \((.*?)\)", string.Empty));

        if (imageInScene.name != "Camera")
        {
            XmlElement ielement = xml.CreateElement("Image"); //Create a new node from scratch
            ielement.SetAttribute("X", (imageInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            ielement.SetAttribute("Y", (-imageInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            ielement.SetAttribute("ClassName", Regex.Replace(imageInScene.name, @" \((.*?)\)", string.Empty)); //Add a name
            SpriteRenderer spriteRenderer = imageInScene.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Image Size in Width and Height
            {

                Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                Vector3 scale = imageInScene.transform.localScale; // Get the GameObject scale
                string sortingLayer = spriteRenderer.sortingLayerName;

                // Retrieve the image resolution of the sprite
                float width = bounds.size.x * 100;
                float height = bounds.size.y * 100;

                // Set the width and height accordingly to the scale in the editor
                ielement.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                ielement.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image

                // Set the Native resolution of sprite
                ielement.SetAttribute("NativeX", width.ToString()); //Native Resolution of the Image in X
                ielement.SetAttribute("NativeY", height.ToString()); //Native Resolution of the Image in Y
                ielement.SetAttribute("Layer", (sortingLayer).ToString());

                // Check the rotation
                {
                    // Convert the rotation to the Marmalade transformation matrix
                    float A, B, C, D, Tx, Ty;
                    ConvertToMarmaladeMatrix(imageInScene, width * scale.x, height * scale.y, out A, out B, out C, out D, out Tx, out Ty);

                    XmlElement matrixElement = xml.CreateElement("Matrix");
                    matrixElement.SetAttribute("A", A.ToString());
                    matrixElement.SetAttribute("B", B.ToString());
                    matrixElement.SetAttribute("C", C.ToString());
                    matrixElement.SetAttribute("D", D.ToString());
                    matrixElement.SetAttribute("Tx", Tx.ToString());
                    matrixElement.SetAttribute("Ty", Ty.ToString());

                    XmlElement propertiesElement = xml.CreateElement("Properties");
                    XmlElement staticElement = xml.CreateElement("Static");
                    staticElement.AppendChild(matrixElement);
                    propertiesElement.AppendChild(staticElement);
                    ielement.AppendChild(propertiesElement);

                    Color color = spriteRenderer.color;
                    if (color.r != 1.000 || color.g != 1.000 || color.b != 1.000)
                    {
                        XmlElement colorElement = xml.CreateElement("StartColor");
                        colorElement.SetAttribute("Color", "#" + ColorUtility.ToHtmlStringRGB(color).ToString());
                        staticElement.AppendChild(colorElement);
                    }

                    if (Regex.Replace(imageInScene.name, @" \((.*?)\)", string.Empty) == "traps_shadows.gradient")
                    {
                        XmlElement blendElement = xml.CreateElement("BlendMode");
                        blendElement.SetAttribute("Mode", "Multiply");
                        staticElement.AppendChild(blendElement);
                    }
                    if (Regex.Replace(imageInScene.name, @" \((.*?)\)", string.Empty) == "traps_shadows.gradient_rounded")
                    {
                        XmlElement blendElement = xml.CreateElement("BlendMode");
                        blendElement.SetAttribute("Mode", "Multiply");
                        staticElement.AppendChild(blendElement);
                    }
                    if (Regex.Replace(imageInScene.name, @" \((.*?)\)", string.Empty) == "traps_shadows.gradient_intense")
                    {
                        XmlElement blendElement = xml.CreateElement("BlendMode");
                        blendElement.SetAttribute("Mode", "Multiply");
                        staticElement.AppendChild(blendElement);
                    }
                    if (Regex.Replace(imageInScene.name, @" \((.*?)\)", string.Empty) == "traps_shadows.gradient_intense_rounded")
                    {
                        XmlElement blendElement = xml.CreateElement("BlendMode");
                        blendElement.SetAttribute("Mode", "Multiply");
                        staticElement.AppendChild(blendElement);
                    }

                }
            }

            node.AppendChild(ielement); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }
    }

    private void ConvertToMarmaladeMatrix(GameObject obj, float width, float height, out float A, out float B, out float C, out float D, out float Tx, out float Ty)
    {
        // Get the rotation in degree
        Vector3 rotation = obj.transform.eulerAngles;

        // Convert to radians
        float thetaZ = rotation.z * Mathf.Deg2Rad;

        // Calculate the matrix elements
        float cosZ = Mathf.Cos(thetaZ);
        float sinZ = Mathf.Sin(thetaZ);

        // spriteRenderer component
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

        // apply flipping
        float flipX = (spriteRenderer != null && spriteRenderer.flipX) ? -1.0f : 1.0f;
        float flipY = (spriteRenderer != null && spriteRenderer.flipY) ? -1.0f : 1.0f;

        // calculation
        A = cosZ * width * flipX;
        B = -sinZ * width * flipX;
        C = sinZ * height * flipY;
        D = cosZ * height * flipY;

        // Tx and Ty are 0 if no rotation
        Tx = 0;
        Ty = 0;
    }


    void ConvertToAnimation(XmlNode node, XmlDocument xml, GameObject animationInScene)
    {
        AnimationProperties AnimationComponent = animationInScene.GetComponent<AnimationProperties>(); // Animation Properties Component

        if (animationInScene.name != "Camera")
        {
            XmlElement animationElement = xml.CreateElement("Animation"); //Create a new node from scratch
            animationElement.SetAttribute("X", (animationInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            animationElement.SetAttribute("Y", (-animationInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            animationElement.SetAttribute("Width", AnimationComponent.Width); //Add a Width
            animationElement.SetAttribute("Height", AnimationComponent.Height); //Add a Height
            animationElement.SetAttribute("Type", AnimationComponent.Type); //Type (default: 1)


            if (!string.IsNullOrEmpty(AnimationComponent.Direction))
            {
                animationElement.SetAttribute("Direction", AnimationComponent.Direction); //Direction (ex: Direction="10|-1.5")
            }

            if (!string.IsNullOrEmpty(AnimationComponent.Acceleration))
            {
                animationElement.SetAttribute("Acceleration", AnimationComponent.Acceleration); //Acceleration (ex: Acceleration="0.02|-0.1")
            }


            animationElement.SetAttribute("ScaleX", AnimationComponent.ScaleX); //Add a ScaleX
            animationElement.SetAttribute("ScaleY", AnimationComponent.ScaleY); //Add a ScaleY

            if (!string.IsNullOrEmpty(AnimationComponent.Time))
            {
                animationElement.SetAttribute("Time", AnimationComponent.Time); //Add a Time
            }

            animationElement.SetAttribute("ClassName", Regex.Replace(animationInScene.name, @" \((.*?)\)", string.Empty)); //Add a name
            node.AppendChild(animationElement); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }
    }

    void ConvertToObjectReference(XmlNode node, XmlDocument xml, GameObject ObjectReferenceTagInScene)
    {
        if (ObjectReferenceTagInScene.name != "Camera")
        {
            ObjectReference objectReference = ObjectReferenceTagInScene.GetComponent<ObjectReference>();
            XmlElement RefElement = xml.CreateElement("ObjectReference"); //Create a new node from scratch
            RefElement.SetAttribute("Name", Regex.Replace(ObjectReferenceTagInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
            RefElement.SetAttribute("X", (ObjectReferenceTagInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            RefElement.SetAttribute("Y", (-ObjectReferenceTagInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            RefElement.SetAttribute("Filename", objectReference.FileName.ToString() + ".xml"); //Add an name


            if (objectReference.useCustomVariables)
            {
                XmlElement contentElement = xml.CreateElement("Properties");
                XmlElement staticElement = xml.CreateElement("Static");

                //xml doesn't format correctly so we load them into a separate doc
                XmlDocument tempDoc = new XmlDocument();
                tempDoc.LoadXml("<Properties>" + objectReference.CustomVariables + "</Properties>");
                foreach (XmlNode childNode in tempDoc.DocumentElement.ChildNodes)
                {
                    XmlNode importedNode = xml.ImportNode(childNode, true);
                    contentElement.AppendChild(importedNode);
                    staticElement.AppendChild(contentElement);
                }

                RefElement.AppendChild(contentElement);
            }

            if (ObjectReferenceTagInScene.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer spriteRenderer = ObjectReferenceTagInScene.GetComponent<SpriteRenderer>();
                Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                Vector3 scale = ObjectReferenceTagInScene.transform.localScale; // Get the GameObject scale

                // Retrieve the image resolution of the sprite
                float width = bounds.size.x * 100;
                float height = bounds.size.y * 100;

                // Convert the rotation to the Marmalade transformation matrix
                float A, B, C, D, Tx, Ty;
                ConvertToMarmaladeMatrix(ObjectReferenceTagInScene, scale.x, scale.y, out A, out B, out C, out D, out Tx, out Ty);

                XmlElement matrixElement = xml.CreateElement("Matrix");
                matrixElement.SetAttribute("A", A.ToString());
                matrixElement.SetAttribute("B", B.ToString());
                matrixElement.SetAttribute("C", C.ToString());
                matrixElement.SetAttribute("D", D.ToString());
                matrixElement.SetAttribute("Tx", Tx.ToString());
                matrixElement.SetAttribute("Ty", Ty.ToString());

                XmlElement propertiesElement = xml.CreateElement("Properties");
                XmlElement staticElement = xml.CreateElement("Static");
                staticElement.AppendChild(matrixElement);
                propertiesElement.AppendChild(staticElement);
                RefElement.AppendChild(propertiesElement);
            }
            node.AppendChild(RefElement); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }


    }


    void ConvertToObject(XmlNode node, XmlDocument xml, GameObject objectInScene)
    {
        //Debug in log every object it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(objectInScene.name, @" \((.*?)\)", string.Empty));

        if (objectInScene.name != "Camera")
        {
            XmlElement element = xml.CreateElement("Object"); //Create a new node from scratch
            element.SetAttribute("Name", Regex.Replace(objectInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
            element.SetAttribute("X", (objectInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            element.SetAttribute("Y", (-objectInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            node.AppendChild(element); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }


        else if (objectInScene.name == "Camera")
        {
            XmlElement element = xml.CreateElement("Camera"); //Create a new node from scratch
            element.SetAttribute("X", (objectInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            element.SetAttribute("Y", (-objectInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            node.AppendChild(element); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }

    }

    void ConvertToPlatform(XmlNode node, XmlDocument xml, GameObject platformInScene) // Platform Collision (Invisible block that is collide-able)
    {
        //Debug in log every platform it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(platformInScene.name, @" \((.*?)\)", string.Empty));

        if (platformInScene.name != "Camera")
        {
            XmlElement P_element = xml.CreateElement("Platform"); //Create a new node from scratch
            P_element.SetAttribute("X", (platformInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            P_element.SetAttribute("Y", (-platformInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

            SpriteRenderer spriteRenderer = platformInScene.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
            {

                Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                Vector3 scale = platformInScene.transform.localScale; // Get the GameObject scale

                // Retrieve the image resolution of the sprite
                float width = bounds.size.x * 100;
                float height = bounds.size.y * 100;

                // Set the width and height accordingly to the scale in the editor
                P_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                P_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image

            }
            node.AppendChild(P_element); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }

    }

    void ConvertToTrapezoid(XmlNode node, XmlDocument xml, GameObject trapezoidInScene) // Trapezoid Collision (Slope)
    {
        //Debug in log every platform it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(trapezoidInScene.name, @" \((.*?)\)", string.Empty));

        if (Regex.Replace(trapezoidInScene.name, @" \((.*?)\)", string.Empty) == "trapezoid_type1") // Slope Default
        {
            XmlElement T_element = xml.CreateElement("Trapezoid"); //Create a new node from scratch
            T_element.SetAttribute("X", (trapezoidInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            T_element.SetAttribute("Y", (-trapezoidInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

            SpriteRenderer spriteRenderer = trapezoidInScene.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
            {

                Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                Vector3 scale = trapezoidInScene.transform.localScale; // Get the GameObject scale

                // Retrieve the image resolution of the sprite
                float width = bounds.size.x * 100;
                float height = bounds.size.y * 100;

                // Set the width and height accordingly to the scale in the editor
                T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Trapezoid
                T_element.SetAttribute("Height", (height * scale.y + 1).ToString()); //Height1 of the Trapezoid

            }
            T_element.SetAttribute("Type", "1"); //Type of the Trapezoid

            node.AppendChild(T_element); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }

        else if (Regex.Replace(trapezoidInScene.name, @" \((.*?)\)", string.Empty) == "trapezoid_type2") // Slope Mirrored
        {
            XmlElement T_element = xml.CreateElement("Trapezoid"); //Create a new node from scratch
            T_element.SetAttribute("X", (trapezoidInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            T_element.SetAttribute("Y", (-trapezoidInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

            SpriteRenderer spriteRenderer = trapezoidInScene.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
            {

                Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                Vector3 scale = trapezoidInScene.transform.localScale; // Get the GameObject scale

                // Retrieve the image resolution of the sprite
                float width = bounds.size.x * 100;
                float height = bounds.size.y * 100;

                // Set the width and height accordingly to the scale in the editor
                T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Trapezoid
                T_element.SetAttribute("Height", (height * scale.y + 1).ToString()); //Height of the Trapezoid

            }
            T_element.SetAttribute("Type", "2"); //Type of the Trapezoid

            node.AppendChild(T_element); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }
    }

    void ConvertToTrigger(XmlNode node, XmlDocument xml, GameObject triggerInScene)
    {
        //Debug in log every trigger it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(triggerInScene.name, @" \((.*?)\)", string.Empty));

        if (triggerInScene.name != "Camera")
        {
            if (triggerInScene.GetComponent<TriggerSettings>() != null) //Checks if the trigger has a setting component
            {
                XmlElement T_element = xml.CreateElement("Trigger"); //Create a new node from scratch
                TriggerSettings triggerSettings = triggerInScene.GetComponent<TriggerSettings>(); //Trigger Settings.cs
                T_element.SetAttribute("Name", Regex.Replace(triggerInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
                T_element.SetAttribute("X", (triggerInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                T_element.SetAttribute("Y", (-triggerInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

                SpriteRenderer spriteRenderer = triggerInScene.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
                {

                    Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                    Vector3 scale = triggerInScene.transform.localScale; // Get the GameObject scale

                    // Retrieve the image resolution of the sprite
                    float width = bounds.size.x * 100;
                    float height = bounds.size.y * 100;

                    // Set the width and height accordingly to the scale in the editor
                    T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                    T_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image

                    // Create the content node and add it to the trigger node
                    XmlElement contentElement = xml.CreateElement("Content");

                    //xml doesn't format correctly so we load them into a separate doc
                    XmlDocument tempDoc = new XmlDocument();
                    tempDoc.LoadXml("<Content>" + triggerSettings.Content + "</Content>");
                    foreach (XmlNode childNode in tempDoc.DocumentElement.ChildNodes)
                    {
                        XmlNode importedNode = xml.ImportNode(childNode, true);
                        contentElement.AppendChild(importedNode);
                    }

                    T_element.AppendChild(contentElement);

                    node.AppendChild(T_element); //Place it into the Object node

                }
            }
            else //continues as normal without any setting attached
            {
                XmlElement T_element = xml.CreateElement("Trigger"); //Create a new node from scratch
                T_element.SetAttribute("Name", Regex.Replace(triggerInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
                T_element.SetAttribute("X", (triggerInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                T_element.SetAttribute("Y", (-triggerInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

                SpriteRenderer spriteRenderer = triggerInScene.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
                {

                    Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                    Vector3 scale = triggerInScene.transform.localScale; // Get the GameObject scale

                    // Retrieve the image resolution of the sprite
                    float width = bounds.size.x * 100;
                    float height = bounds.size.y * 100;

                    // Set the width and height accordingly to the scale in the editor
                    T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                    T_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image
                    node.AppendChild(T_element); //Place it into the Object node
                }
            }

            //apply the modification to the build-map.xml with proper format
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml", settings))
            {
                xml.Save(writer);
            }
        }
    }
    //  ^^^ ExtractAttributeValue is for the method above ^^^
    private string ExtractAttributeValue(string line, string attributeName)
    {
        int startIndex = line.IndexOf(attributeName + "=\"") + (attributeName + "=\"").Length;
        int endIndex = line.IndexOf("\"", startIndex);
        if (startIndex != -1 && endIndex != -1)
        {
            return line.Substring(startIndex, endIndex - startIndex);
        }
        return null;
    }


    void ConvertToDynamicTrigger(XmlNode node, XmlDocument xml, GameObject dynamictriggerInScene)
    {
        DynamicTrigger dynamicTrigger = dynamictriggerInScene.GetComponent<DynamicTrigger>();
        XmlElement T_element = xml.CreateElement("Trigger");
        T_element.SetAttribute("Name", "");
        T_element.SetAttribute("Name", Regex.Replace(dynamictriggerInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
        T_element.SetAttribute("X", (dynamictriggerInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
        T_element.SetAttribute("Y", (-dynamictriggerInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
        SpriteRenderer spriteRenderer = dynamictriggerInScene.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
        {

            Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
            Vector3 scale = dynamictriggerInScene.transform.localScale; // Get the GameObject scale

            // Retrieve the image resolution of the sprite
            float width = bounds.size.x * 100;
            float height = bounds.size.y * 100;

            // Set the width and height accordingly to the scale in the editor
            T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
            T_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image
        }

        // Create Init element
        XmlElement initElement = xml.CreateElement("Init");

        // Add SetVariable elements to Init
        XmlElement setVariable1 = xml.CreateElement("SetVariable");
        setVariable1.SetAttribute("Name", "$Active");
        setVariable1.SetAttribute("Value", "1");
        initElement.AppendChild(setVariable1);

        XmlElement setVariable2 = xml.CreateElement("SetVariable");
        setVariable2.SetAttribute("Name", "$AI");
        setVariable2.SetAttribute("Value", dynamicTrigger.AIAllowed.ToString());
        initElement.AppendChild(setVariable2);

        XmlElement setVariable3 = xml.CreateElement("SetVariable");
        setVariable3.SetAttribute("Name", "$Node");
        setVariable3.SetAttribute("Value", "COM");
        initElement.AppendChild(setVariable3);

        if (dynamicTrigger.PlaySound)
        {
            XmlElement setVariable4 = xml.CreateElement("SetVariable");
            setVariable4.SetAttribute("Name", "Sound");
            setVariable4.SetAttribute("Value", dynamicTrigger.Sound);
            initElement.AppendChild(setVariable4);
        }

        XmlElement setVariable5 = xml.CreateElement("SetVariable");
        setVariable5.SetAttribute("Name", "Flag1");
        setVariable5.SetAttribute("Value", "0");
        initElement.AppendChild(setVariable5);

        // Create Trigger Content element
        XmlElement triggerContentElement = xml.CreateElement("Content");

        // Append Init to Content
        triggerContentElement.AppendChild(initElement);

        // Create Loop element
        XmlElement loopElement = xml.CreateElement("Loop");

        // Create Events element and EventBlock element
        XmlElement eventsElement = xml.CreateElement("Events");
        XmlElement eventBlockElement = xml.CreateElement("EventBlock");
        eventBlockElement.SetAttribute("Template", "FreqUsed.Enter");
        eventsElement.AppendChild(eventBlockElement);

        // Append Events to Loop
        loopElement.AppendChild(eventsElement);

        // Create Actions element and ActionBlock element
        XmlElement actionsElement = xml.CreateElement("Actions");
        XmlElement actionBlockElement = xml.CreateElement("ActionBlock");
        actionBlockElement.SetAttribute("Template", "FreqUsed.SwitchOff");
        actionsElement.AppendChild(actionBlockElement);

        // Create Transform element and append to Loop
        XmlElement transformElement = xml.CreateElement("Transform");
        transformElement.SetAttribute("Name", dynamicTrigger.TriggerTransformName);
        actionsElement.AppendChild(transformElement);

        if (dynamicTrigger.PlaySound)
        {
            // Create Actionsblock sound
            XmlElement actionBlockSoundElement = xml.CreateElement("ActionBlock");
            actionBlockSoundElement.SetAttribute("Template", "CommonLib.Sound");
            actionsElement.AppendChild(actionBlockSoundElement);
        }

        // Append Actions to Loop
        loopElement.AppendChild(actionsElement);

        // Append Loop to Trigger
        triggerContentElement.AppendChild(loopElement);

        // Append Content to Trigger
        T_element.AppendChild(triggerContentElement);
        node.AppendChild(T_element); //Place it into the Object node
        XmlWriterSettings settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };

        using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml", settings))
        {
            xml.Save(writer);
        }

    }

    void ConvertToArea(XmlNode node, XmlDocument xml, GameObject areaInScene)
    {
        //Debug in log every Area it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(areaInScene.name, @" \((.*?)\)", string.Empty));

        if (areaInScene.name != "Camera")
        {
            XmlElement A_element = xml.CreateElement("Area"); //Create a new node from scratch
            A_element.SetAttribute("Name", Regex.Replace(areaInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
            A_element.SetAttribute("X", (areaInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            A_element.SetAttribute("Y", (-areaInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

            SpriteRenderer spriteRenderer = areaInScene.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
            {

                Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                Vector3 scale = areaInScene.transform.localScale; // Get the GameObject scale

                // Retrieve the image resolution of the sprite
                float width = bounds.size.x * 100;
                float height = bounds.size.y * 100;

                // Set the width and height accordingly to the scale in the editor
                A_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                A_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image

            }
            A_element.SetAttribute("Type", "Animation"); //Type="Animation"/>
            node.AppendChild(A_element); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }
    }


    void ConvertToIn(XmlNode node, XmlDocument xml, GameObject InTagInScene)
    {

        if (InTagInScene.name != "Camera")
        {
            XmlElement A_element = xml.CreateElement("In"); //Create a new node from scratch
            A_element.SetAttribute("Name", Regex.Replace(InTagInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
            A_element.SetAttribute("X", (InTagInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            A_element.SetAttribute("Y", (-InTagInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)


            node.AppendChild(A_element); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }
    }

    void ConvertToOut(XmlNode node, XmlDocument xml, GameObject OutTagInScene)
    {

        if (OutTagInScene.name != "Camera")
        {
            XmlElement A_element = xml.CreateElement("Out"); //Create a new node from scratch
            A_element.SetAttribute("Name", Regex.Replace(OutTagInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
            A_element.SetAttribute("X", (OutTagInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            A_element.SetAttribute("Y", (-OutTagInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)


            node.AppendChild(A_element); //Place it into the Object node
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }
    }

    void ConvertToCamera(XmlNode node, XmlDocument xml, GameObject camInScene)
    {


        //Important Note: If the specific TriggerZoom already exists in the object.xml, no need to tag those as Camera. Instead, tag it as an object!



        // Debug in log every Area it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(camInScene.name, @" \((.*?)\)", string.Empty));


        if (camInScene.name != "Camera") //kinda ironic
        {
            SpriteRenderer spriteRenderer = camInScene.GetComponent<SpriteRenderer>();
            CustomZoom customZoomValue = camInScene.GetComponent<CustomZoom>(); //Zoom value from object with tag "Camera" that have CustomZoom component
            Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
            Vector3 scale = camInScene.transform.localScale; // Get the GameObject scale
            // Retrieve the image resolution of the sprite
            float width = bounds.size.x * 100;
            float height = bounds.size.y * 100;

            //Trigger Childs
            XmlElement contentElement = xml.CreateElement("Content");
            XmlElement initElement = xml.CreateElement("Init");

            //trigger variable
            string[] variableNames = { "$Active", "$Node", "Zoom", "$AI", "Flag1" };
            string[] variableValues = { "1", "COM", customZoomValue.ZoomAmount.ToString(), "0", "0" };


            XmlElement triggerElement = xml.CreateElement("Trigger");
            triggerElement.SetAttribute("Name", Regex.Replace(camInScene.name, @" \((.*?)\)", string.Empty));
            triggerElement.SetAttribute("X", (camInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            triggerElement.SetAttribute("Y", (-camInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            triggerElement.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
            triggerElement.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image

            //writes <content> and <init> under the trigger node
            for (int i = 0; i < variableNames.Length; i++)
            {
                XmlElement setVariableElement = xml.CreateElement("SetVariable");
                setVariableElement.SetAttribute("Name", variableNames[i]);
                setVariableElement.SetAttribute("Value", variableValues[i]);
                initElement.AppendChild(setVariableElement);
            }

            XmlElement templateElement = xml.CreateElement("Template");
            templateElement.SetAttribute("Name", "CameraZoom");

            // Append elements
            contentElement.AppendChild(initElement);
            contentElement.AppendChild(templateElement);
            triggerElement.AppendChild(contentElement);

            // Append the Trigger element to the XmlDocument
            node.AppendChild(triggerElement);
            xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
        }
    }

    void ConvertToDynamic(XmlNode node, XmlDocument xml, GameObject dynamicInScene, UnityEngine.Transform dynamicInSceneTransform)
    {
        // Dynamic component in the hierachy
        DynamicVec2 dynamicComponent = dynamicInScene.GetComponent<DynamicVec2>();
        XmlElement objectElement = xml.CreateElement("Object");


        if (dynamicComponent.MovementUsage.IsSizeInterval)
        {
            objectElement.SetAttribute("X", (dynamicInScene.transform.position.x * 100).ToString().Replace(',', '.'));
            objectElement.SetAttribute("Y", (-dynamicInScene.transform.position.y * 100).ToString().Replace(',', '.'));
        }

        if (!dynamicComponent.MovementUsage.IsSizeInterval)
        {
            objectElement.SetAttribute("X", "0");
            objectElement.SetAttribute("Y", "0");
        }
        // Properties
        XmlElement propertiesElement = xml.CreateElement("Properties");
        if (dynamicComponent.MovementUsage.IsSizeInterval)
        {
            XmlElement staticElement = xml.CreateElement("Static");

            //xml doesn't format correctly so we load them into a separate doc
            XmlDocument tempDoc = new XmlDocument();
            tempDoc.LoadXml("<Properties>" + dynamicComponent.MovementUsage.matrixProperties + "</Properties>");
            foreach (XmlNode childNode in tempDoc.DocumentElement.ChildNodes)
            {
                XmlNode importedNode = xml.ImportNode(childNode, true);
                staticElement.AppendChild(importedNode);
            }

            propertiesElement.AppendChild(staticElement);
        }

        // Dynamic
        XmlElement dynamicElement = xml.CreateElement("Dynamic");

        // Create Transformation element
        XmlElement transformationElement = xml.CreateElement("Transformation");
        transformationElement.SetAttribute("Name", dynamicComponent.TransformationName);


        // Move Interval 1
        if (dynamicComponent.MovementUsage.UseMovement1)
        {
            if (dynamicComponent.MovementUsage.IsSizeInterval)
            {
                if (dynamicComponent.MoveInterval1.UseDelay)
                {
                    XmlElement delayElement = xml.CreateElement("DelayInterval");
                    delayElement.SetAttribute("Frames", (dynamicComponent.MoveInterval1.Delay * 60).ToString()); //multiply second by 60 frames per second
                    transformationElement.AppendChild(delayElement);
                }
                XmlElement moveIntervalElement = xml.CreateElement("SizeInterval");
                moveIntervalElement.SetAttribute("Frames", (dynamicComponent.MoveInterval1.MoveDuration * 60).ToString()); //multiply second by 60 frames per second

                // Create Points (Start, Support, Finish)
                XmlElement startPointElement = xml.CreateElement("Point");
                startPointElement.SetAttribute("W", "1");
                startPointElement.SetAttribute("H", "1");

                XmlElement finishPointElement = xml.CreateElement("Point");
                finishPointElement.SetAttribute("W", (dynamicComponent.MoveInterval1.ReziseXAxis * 100).ToString());
                finishPointElement.SetAttribute("H", (dynamicComponent.MoveInterval1.ReziseYAxis * 100).ToString());

                // Append points to MoveInterval
                moveIntervalElement.AppendChild(startPointElement);
                moveIntervalElement.AppendChild(finishPointElement);

                transformationElement.AppendChild(moveIntervalElement);
            }
            else if (!dynamicComponent.MovementUsage.IsSizeInterval)
            {
                if (dynamicComponent.MoveInterval1.UseDelay)
                {
                    XmlElement delayElement = xml.CreateElement("DelayInterval");
                    delayElement.SetAttribute("Frames", (dynamicComponent.MoveInterval1.Delay * 60).ToString()); //multiply second by 60 frames per second
                    transformationElement.AppendChild(delayElement);
                }
                XmlElement moveIntervalElement = xml.CreateElement("MoveInterval");
                moveIntervalElement.SetAttribute("Frames", (dynamicComponent.MoveInterval1.MoveDuration * 60).ToString()); //multiply second by 60 frames per second

                // Create Points (Start, Support, Finish)
                XmlElement startPointElement = xml.CreateElement("Point");
                startPointElement.SetAttribute("X", "0.0");
                startPointElement.SetAttribute("Y", "0.0");

                XmlElement supportPointElement = xml.CreateElement("Point");
                supportPointElement.SetAttribute("X", (dynamicComponent.MoveInterval1.SupportXAxis * 100).ToString());
                supportPointElement.SetAttribute("Y", (-dynamicComponent.MoveInterval1.SupportYAxis * 100).ToString());

                XmlElement finishPointElement = xml.CreateElement("Point");
                finishPointElement.SetAttribute("X", (dynamicComponent.MoveInterval1.MoveXAxis * 100).ToString());
                finishPointElement.SetAttribute("Y", (-dynamicComponent.MoveInterval1.MoveYAxis * 100).ToString());

                // Append points to MoveInterval
                moveIntervalElement.AppendChild(startPointElement);
                moveIntervalElement.AppendChild(supportPointElement);
                moveIntervalElement.AppendChild(finishPointElement);

                transformationElement.AppendChild(moveIntervalElement);
            }
        }

        // Move Interval 2
        if (dynamicComponent.MovementUsage.UseMovement2)
        {
            if (dynamicComponent.MovementUsage.IsSizeInterval)
            {
                if (dynamicComponent.MoveInterval2.UseDelay)
                {
                    XmlElement delayElement = xml.CreateElement("DelayInterval");
                    delayElement.SetAttribute("Frames", (dynamicComponent.MoveInterval2.Delay * 60).ToString()); //multiply second by 60 frames per second
                    transformationElement.AppendChild(delayElement);
                }
                XmlElement moveIntervalElement = xml.CreateElement("SizeInterval");
                moveIntervalElement.SetAttribute("Frames", (dynamicComponent.MoveInterval2.MoveDuration * 60).ToString()); //multiply second by 60 frames per second

                // Create Points (Start, Support, Finish)
                XmlElement startPointElement = xml.CreateElement("Point");
                startPointElement.SetAttribute("W", "1");
                startPointElement.SetAttribute("H", "1");

                XmlElement finishPointElement = xml.CreateElement("Point");
                finishPointElement.SetAttribute("W", (dynamicComponent.MoveInterval2.ReziseXAxis * 100).ToString());
                finishPointElement.SetAttribute("H", (dynamicComponent.MoveInterval2.ReziseYAxis * 100).ToString());

                // Append points to MoveInterval
                moveIntervalElement.AppendChild(startPointElement);
                moveIntervalElement.AppendChild(finishPointElement);

                transformationElement.AppendChild(moveIntervalElement);
            }
            else if (!dynamicComponent.MovementUsage.IsSizeInterval)
            {
                if (dynamicComponent.MoveInterval2.UseDelay)
                {
                    XmlElement delayElement = xml.CreateElement("DelayInterval");
                    delayElement.SetAttribute("Frames", (dynamicComponent.MoveInterval2.Delay * 60).ToString()); //multiply second by 60 frames per second
                    transformationElement.AppendChild(delayElement);
                }
                XmlElement moveIntervalElement = xml.CreateElement("MoveInterval");
                moveIntervalElement.SetAttribute("Frames", (dynamicComponent.MoveInterval2.MoveDuration * 60).ToString()); //multiply second by 60 frames per second

                // Create Points (Start, Support, Finish)
                XmlElement startPointElement = xml.CreateElement("Point");
                startPointElement.SetAttribute("X", "0.0");
                startPointElement.SetAttribute("Y", "0.0");

                XmlElement supportPointElement = xml.CreateElement("Point");
                supportPointElement.SetAttribute("X", (dynamicComponent.MoveInterval2.SupportXAxis * 100).ToString());
                supportPointElement.SetAttribute("Y", (-dynamicComponent.MoveInterval2.SupportYAxis * 100).ToString());

                XmlElement finishPointElement = xml.CreateElement("Point");
                finishPointElement.SetAttribute("X", (dynamicComponent.MoveInterval2.MoveXAxis * 100).ToString());
                finishPointElement.SetAttribute("Y", (-dynamicComponent.MoveInterval2.MoveYAxis * 100).ToString());

                // Append points to MoveInterval
                moveIntervalElement.AppendChild(startPointElement);
                moveIntervalElement.AppendChild(supportPointElement);
                moveIntervalElement.AppendChild(finishPointElement);

                transformationElement.AppendChild(moveIntervalElement);
            }
        }

        // Move Interval 3
        if (dynamicComponent.MovementUsage.UseMovement3)
        {
            if (dynamicComponent.MovementUsage.IsSizeInterval)
            {
                if (dynamicComponent.MoveInterval3.UseDelay)
                {
                    XmlElement delayElement = xml.CreateElement("DelayInterval");
                    delayElement.SetAttribute("Frames", (dynamicComponent.MoveInterval3.Delay * 60).ToString()); //multiply second by 60 frames per second
                    transformationElement.AppendChild(delayElement);
                }
                XmlElement moveIntervalElement = xml.CreateElement("SizeInterval");
                moveIntervalElement.SetAttribute("Frames", (dynamicComponent.MoveInterval3.MoveDuration * 60).ToString()); //multiply second by 60 frames per second

                // Create Points (Start, Support, Finish)
                XmlElement startPointElement = xml.CreateElement("Point");
                startPointElement.SetAttribute("W", "1");
                startPointElement.SetAttribute("H", "1");

                XmlElement finishPointElement = xml.CreateElement("Point");
                finishPointElement.SetAttribute("W", (dynamicComponent.MoveInterval3.ReziseXAxis * 100).ToString());
                finishPointElement.SetAttribute("H", (dynamicComponent.MoveInterval3.ReziseYAxis * 100).ToString());

                // Append points to MoveInterval
                moveIntervalElement.AppendChild(startPointElement);
                moveIntervalElement.AppendChild(finishPointElement);

                transformationElement.AppendChild(moveIntervalElement);
            }
            else if (!dynamicComponent.MovementUsage.IsSizeInterval)
            {
                if (dynamicComponent.MoveInterval3.UseDelay)
                {
                    XmlElement delayElement = xml.CreateElement("DelayInterval");
                    delayElement.SetAttribute("Frames", (dynamicComponent.MoveInterval3.Delay * 60).ToString()); //multiply second by 60 frames per second
                    transformationElement.AppendChild(delayElement);
                }
                XmlElement moveIntervalElement = xml.CreateElement("MoveInterval");
                moveIntervalElement.SetAttribute("Frames", (dynamicComponent.MoveInterval3.MoveDuration * 60).ToString()); //multiply second by 60 frames per second

                // Create Points (Start, Support, Finish)
                XmlElement startPointElement = xml.CreateElement("Point");
                startPointElement.SetAttribute("X", "0.0");
                startPointElement.SetAttribute("Y", "0.0");

                XmlElement supportPointElement = xml.CreateElement("Point");
                supportPointElement.SetAttribute("X", (dynamicComponent.MoveInterval3.SupportXAxis * 100).ToString());
                supportPointElement.SetAttribute("Y", (-dynamicComponent.MoveInterval3.SupportYAxis * 100).ToString());

                XmlElement finishPointElement = xml.CreateElement("Point");
                finishPointElement.SetAttribute("X", (dynamicComponent.MoveInterval3.MoveXAxis * 100).ToString());
                finishPointElement.SetAttribute("Y", (-dynamicComponent.MoveInterval3.MoveYAxis * 100).ToString());

                // Append points to MoveInterval
                moveIntervalElement.AppendChild(startPointElement);
                moveIntervalElement.AppendChild(supportPointElement);
                moveIntervalElement.AppendChild(finishPointElement);

                transformationElement.AppendChild(moveIntervalElement);
            }
        }

        // Move Interval 4
        if (dynamicComponent.MovementUsage.UseMovement4)
        {
            if (dynamicComponent.MovementUsage.IsSizeInterval)
            {
                if (dynamicComponent.MoveInterval4.UseDelay)
                {
                    XmlElement delayElement = xml.CreateElement("DelayInterval");
                    delayElement.SetAttribute("Frames", (dynamicComponent.MoveInterval4.Delay * 60).ToString()); //multiply second by 60 frames per second
                    transformationElement.AppendChild(delayElement);
                }
                XmlElement moveIntervalElement = xml.CreateElement("SizeInterval");
                moveIntervalElement.SetAttribute("Frames", (dynamicComponent.MoveInterval4.MoveDuration * 60).ToString()); //multiply second by 60 frames per second

                // Create Points (Start, Support, Finish)
                XmlElement startPointElement = xml.CreateElement("Point");
                startPointElement.SetAttribute("W", "1");
                startPointElement.SetAttribute("H", "1");

                XmlElement finishPointElement = xml.CreateElement("Point");
                finishPointElement.SetAttribute("W", (dynamicComponent.MoveInterval4.ReziseXAxis * 100).ToString());
                finishPointElement.SetAttribute("H", (dynamicComponent.MoveInterval4.ReziseYAxis * 100).ToString());

                // Append points to MoveInterval
                moveIntervalElement.AppendChild(startPointElement);
                moveIntervalElement.AppendChild(finishPointElement);

                transformationElement.AppendChild(moveIntervalElement);
            }
            else if (!dynamicComponent.MovementUsage.IsSizeInterval)
            {
                if (dynamicComponent.MoveInterval4.UseDelay)
                {
                    XmlElement delayElement = xml.CreateElement("DelayInterval");
                    delayElement.SetAttribute("Frames", (dynamicComponent.MoveInterval4.Delay * 60).ToString()); //multiply second by 60 frames per second
                    transformationElement.AppendChild(delayElement);
                }
                XmlElement moveIntervalElement = xml.CreateElement("MoveInterval");
                moveIntervalElement.SetAttribute("Frames", (dynamicComponent.MoveInterval4.MoveDuration * 60).ToString()); //multiply second by 60 frames per second

                // Create Points (Start, Support, Finish)
                XmlElement startPointElement = xml.CreateElement("Point");
                startPointElement.SetAttribute("X", "0.0");
                startPointElement.SetAttribute("Y", "0.0");

                XmlElement supportPointElement = xml.CreateElement("Point");
                supportPointElement.SetAttribute("X", (dynamicComponent.MoveInterval4.SupportXAxis * 100).ToString());
                supportPointElement.SetAttribute("Y", (-dynamicComponent.MoveInterval4.SupportYAxis * 100).ToString());

                XmlElement finishPointElement = xml.CreateElement("Point");
                finishPointElement.SetAttribute("X", (dynamicComponent.MoveInterval4.MoveXAxis * 100).ToString());
                finishPointElement.SetAttribute("Y", (-dynamicComponent.MoveInterval4.MoveYAxis * 100).ToString());

                // Append points to MoveInterval
                moveIntervalElement.AppendChild(startPointElement);
                moveIntervalElement.AppendChild(supportPointElement);
                moveIntervalElement.AppendChild(finishPointElement);

                transformationElement.AppendChild(moveIntervalElement);
            }
        }

        // Move Interval 5
        if (dynamicComponent.MovementUsage.UseMovement5)
        {
            if (dynamicComponent.MovementUsage.IsSizeInterval)
            {
                if (dynamicComponent.MoveInterval5.UseDelay)
                {
                    XmlElement delayElement = xml.CreateElement("DelayInterval");
                    delayElement.SetAttribute("Frames", (dynamicComponent.MoveInterval5.Delay * 60).ToString()); //multiply second by 60 frames per second
                    transformationElement.AppendChild(delayElement);
                }
                XmlElement moveIntervalElement = xml.CreateElement("SizeInterval");
                moveIntervalElement.SetAttribute("Frames", (dynamicComponent.MoveInterval5.MoveDuration * 60).ToString()); //multiply second by 60 frames per second

                // Create Points (Start, Support, Finish)
                XmlElement startPointElement = xml.CreateElement("Point");
                startPointElement.SetAttribute("W", "1");
                startPointElement.SetAttribute("H", "1");

                XmlElement finishPointElement = xml.CreateElement("Point");
                finishPointElement.SetAttribute("W", (dynamicComponent.MoveInterval5.ReziseXAxis * 100).ToString());
                finishPointElement.SetAttribute("H", (dynamicComponent.MoveInterval5.ReziseYAxis * 100).ToString());

                // Append points to MoveInterval
                moveIntervalElement.AppendChild(startPointElement);
                moveIntervalElement.AppendChild(finishPointElement);

                transformationElement.AppendChild(moveIntervalElement);
            }
            else if (!dynamicComponent.MovementUsage.IsSizeInterval)
            {
                if (dynamicComponent.MoveInterval5.UseDelay)
                {
                    XmlElement delayElement = xml.CreateElement("DelayInterval");
                    delayElement.SetAttribute("Frames", (dynamicComponent.MoveInterval5.Delay * 60).ToString()); //multiply second by 60 frames per second
                    transformationElement.AppendChild(delayElement);
                }
                XmlElement moveIntervalElement = xml.CreateElement("MoveInterval");
                moveIntervalElement.SetAttribute("Frames", (dynamicComponent.MoveInterval5.MoveDuration * 60).ToString()); //multiply second by 60 frames per second

                // Create Points (Start, Support, Finish)
                XmlElement startPointElement = xml.CreateElement("Point");
                startPointElement.SetAttribute("X", "0.0");
                startPointElement.SetAttribute("Y", "0.0");

                XmlElement supportPointElement = xml.CreateElement("Point");
                supportPointElement.SetAttribute("X", (dynamicComponent.MoveInterval5.SupportXAxis * 100).ToString());
                supportPointElement.SetAttribute("Y", (-dynamicComponent.MoveInterval5.SupportYAxis * 100).ToString());

                XmlElement finishPointElement = xml.CreateElement("Point");
                finishPointElement.SetAttribute("X", (dynamicComponent.MoveInterval5.MoveXAxis * 100).ToString());
                finishPointElement.SetAttribute("Y", (-dynamicComponent.MoveInterval5.MoveYAxis * 100).ToString());

                // Append points to MoveInterval
                moveIntervalElement.AppendChild(startPointElement);
                moveIntervalElement.AppendChild(supportPointElement);
                moveIntervalElement.AppendChild(finishPointElement);

                transformationElement.AppendChild(moveIntervalElement);
            }
        }


        dynamicElement.AppendChild(transformationElement);
        propertiesElement.AppendChild(dynamicElement);
        objectElement.AppendChild(propertiesElement);


        // Create Content element
        XmlElement contentElement = xml.CreateElement("Content");

        foreach (UnityEngine.Transform childObject in dynamicInSceneTransform)
        {
            //check if the gameobject has specific tag

            if (childObject.gameObject.CompareTag("Object"))
            {
                if (childObject.name != "Camera")
                {
                    XmlElement objElement = xml.CreateElement("Object"); //Create a new node from scratch
                    objElement.SetAttribute("Name", Regex.Replace(childObject.name, @" \((.*?)\)", string.Empty)); //Add an name
                    objElement.SetAttribute("X", (childObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    objElement.SetAttribute("Y", (-childObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                    contentElement.AppendChild(objElement);
                }
            }
            else if (childObject.gameObject.CompareTag("Object Reference"))
            {
                ObjectReference objectReference = childObject.GetComponent<ObjectReference>();
                XmlElement RefElement = xml.CreateElement("ObjectReference"); //Create a new node from scratch
                RefElement.SetAttribute("Name", Regex.Replace(childObject.name, @" \((.*?)\)", string.Empty)); //Add an name
                RefElement.SetAttribute("X", (childObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                RefElement.SetAttribute("Y", (-childObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                RefElement.SetAttribute("Filename", objectReference.FileName.ToString() + ".xml"); //Add an name


                if (objectReference.useCustomVariables)
                {
                    XmlElement refpropertiesElement = xml.CreateElement("Properties");
                    XmlElement staticElement = xml.CreateElement("Static");

                    //xml doesn't format correctly so we load them into a separate doc
                    XmlDocument tempDoc = new XmlDocument();
                    tempDoc.LoadXml("<Properties>" + objectReference.CustomVariables + "</Properties>");
                    foreach (XmlNode childNode in tempDoc.DocumentElement.ChildNodes)
                    {
                        XmlNode importedNode = xml.ImportNode(childNode, true);
                        refpropertiesElement.AppendChild(importedNode);
                        staticElement.AppendChild(refpropertiesElement);
                    }

                    RefElement.AppendChild(refpropertiesElement);
                }
                contentElement.AppendChild(RefElement);
            }
            else if (childObject.gameObject.CompareTag("Image"))
            {
                XmlElement ielement = xml.CreateElement("Image"); //Create a new node from scratch
                if (dynamicComponent.MovementUsage.IsSizeInterval)
                {
                    ielement.SetAttribute("X", (childObject.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    ielement.SetAttribute("Y", (-childObject.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                }
                if (!dynamicComponent.MovementUsage.IsSizeInterval)
                {
                    ielement.SetAttribute("X", (childObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    ielement.SetAttribute("Y", (-childObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                }
                ielement.SetAttribute("ClassName", Regex.Replace(childObject.name, @" \((.*?)\)", string.Empty)); //Add a name
                SpriteRenderer spriteRenderer = childObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Image Size in Width and Height
                {

                    Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                    Vector3 scale = childObject.transform.localScale; // Get the GameObject scale
                    string sortingLayer = spriteRenderer.sortingLayerName;

                    // Retrieve the image resolution of the sprite
                    float width = bounds.size.x * 100;
                    float height = bounds.size.y * 100;

                    // Set the width and height accordingly to the scale in the editor
                    ielement.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                    ielement.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image

                    // Set the Native resolution of sprite
                    ielement.SetAttribute("NativeX", width.ToString()); //Native Resolution of the Image in X
                    ielement.SetAttribute("NativeY", height.ToString()); //Native Resolution of the Image in Y
                    ielement.SetAttribute("Layer", (sortingLayer).ToString());

                    // Check the rotation
                    {
                        // Convert the rotation to the Marmalade transformation matrix
                        float A, B, C, D, Tx, Ty;
                        ConvertToMarmaladeMatrix(childObject.gameObject, width * scale.x, height * scale.y, out A, out B, out C, out D, out Tx, out Ty);

                        XmlElement matrixElement = xml.CreateElement("Matrix");
                        matrixElement.SetAttribute("A", A.ToString());
                        matrixElement.SetAttribute("B", B.ToString());
                        matrixElement.SetAttribute("C", C.ToString());
                        matrixElement.SetAttribute("D", D.ToString());
                        matrixElement.SetAttribute("Tx", Tx.ToString());
                        matrixElement.SetAttribute("Ty", Ty.ToString());

                        XmlElement propertiesElement1 = xml.CreateElement("Properties");
                        XmlElement staticElement = xml.CreateElement("Static");
                        staticElement.AppendChild(matrixElement);
                        propertiesElement1.AppendChild(staticElement);
                        ielement.AppendChild(propertiesElement1);
                    }


                }
                contentElement.AppendChild(ielement);
            }
            else if (childObject.gameObject.CompareTag("Platform"))
            {
                //Platform
                XmlElement P_element = xml.CreateElement("Platform"); //Create a new node from scratch
                P_element.SetAttribute("X", (childObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                P_element.SetAttribute("Y", (-childObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

                SpriteRenderer spriteRenderer = childObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
                {

                    Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                    Vector3 scale = childObject.transform.localScale; // Get the GameObject scale

                    // Retrieve the image resolution of the sprite
                    float width = bounds.size.x * 100;
                    float height = bounds.size.y * 100;

                    // Set the width and height accordingly to the scale in the editor
                    P_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                    P_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image

                }
                contentElement.AppendChild(P_element);
      
            }
            else if (childObject.gameObject.CompareTag("Trapezoid"))
            {
                // Trapezoid
                if (Regex.Replace(childObject.name, @" \((.*?)\)", string.Empty) == "trapezoid_type1") // Slope Default
                {
                    XmlElement T_element = xml.CreateElement("Trapezoid"); //Create a new node from scratch
                    T_element.SetAttribute("X", (childObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    T_element.SetAttribute("Y", (-childObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

                    SpriteRenderer spriteRenderer = childObject.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
                    {

                        Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                        Vector3 scale = childObject.transform.localScale; // Get the GameObject scale

                        // Retrieve the image resolution of the sprite
                        float width = bounds.size.x * 100;
                        float height = bounds.size.y * 100;

                        // Set the width and height accordingly to the scale in the editor
                        T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Trapezoid
                        T_element.SetAttribute("Height", "1"); //Height of the Trapezoid
                        T_element.SetAttribute("Height1", (height * scale.y + 1).ToString()); //Height1 of the Trapezoid

                    }
                    T_element.SetAttribute("Type", "1"); //Type of the Trapezoid

                    contentElement.AppendChild(T_element);
                }

                else if (Regex.Replace(childObject.name, @" \((.*?)\)", string.Empty) == "trapezoid_type2") // Slope Mirrored
                {
                    XmlElement T_element = xml.CreateElement("Trapezoid"); //Create a new node from scratch
                    T_element.SetAttribute("X", (childObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    T_element.SetAttribute("Y", (-childObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

                    SpriteRenderer spriteRenderer = childObject.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
                    {

                        Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                        Vector3 scale = childObject.transform.localScale; // Get the GameObject scale

                        // Retrieve the image resolution of the sprite
                        float width = bounds.size.x * 100;
                        float height = bounds.size.y * 100;

                        // Set the width and height accordingly to the scale in the editor
                        T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Trapezoid
                        T_element.SetAttribute("Height", (height * scale.y + 1).ToString()); //Height of the Trapezoid
                        T_element.SetAttribute("Height1", "1"); //Height1 of the Trapezoid

                    }
                    T_element.SetAttribute("Type", "2"); //Type of the Trapezoid

                    contentElement.AppendChild(T_element);
                }
            }
            else if (childObject.gameObject.CompareTag("Area"))
            {
                XmlElement A_element = xml.CreateElement("Area"); //Create a new node from scratch
                A_element.SetAttribute("Name", Regex.Replace(childObject.name, @" \((.*?)\)", string.Empty)); //Add an name
                A_element.SetAttribute("X", (childObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                A_element.SetAttribute("Y", (-childObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

                SpriteRenderer spriteRenderer = childObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
                {

                    Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                    Vector3 scale = childObject.transform.localScale; // Get the GameObject scale

                    // Retrieve the image resolution of the sprite
                    float width = bounds.size.x * 100;
                    float height = bounds.size.y * 100;

                    // Set the width and height accordingly to the scale in the editor
                    A_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                    A_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image

                }
                A_element.SetAttribute("Type", "Animation"); //Type="Animation"/>
                contentElement.AppendChild(A_element);
            }
            else if (childObject.gameObject.CompareTag("Trigger"))
            {
                DynamicTrigger dynamicTrigger = childObject.GetComponent<DynamicTrigger>();

                if (dynamicTrigger != null)
                {
                    XmlElement T_element = xml.CreateElement("Trigger");
                    T_element.SetAttribute("Name", "");
                    T_element.SetAttribute("Name", Regex.Replace(childObject.name, @" \((.*?)\)", string.Empty)); //Add an name
                    if (dynamicComponent.MovementUsage.IsSizeInterval)
                    {
                        T_element.SetAttribute("X", (childObject.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                        T_element.SetAttribute("Y", (-childObject.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                    }
                    if (!dynamicComponent.MovementUsage.IsSizeInterval)
                    {
                        T_element.SetAttribute("X", (childObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                        T_element.SetAttribute("Y", (-childObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                    }
                    SpriteRenderer spriteRenderer = childObject.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
                    {

                        Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                        Vector3 scale = childObject.transform.localScale; // Get the GameObject scale

                        // Retrieve the image resolution of the sprite
                        float width = bounds.size.x * 100;
                        float height = bounds.size.y * 100;

                        // Set the width and height accordingly to the scale in the editor
                        T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                        T_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image
                    }

                    // Create Init element
                    XmlElement initElement = xml.CreateElement("Init");

                    // Add SetVariable elements to Init
                    XmlElement setVariable1 = xml.CreateElement("SetVariable");
                    setVariable1.SetAttribute("Name", "$Active");
                    setVariable1.SetAttribute("Value", "1");
                    initElement.AppendChild(setVariable1);

                    XmlElement setVariable2 = xml.CreateElement("SetVariable");
                    setVariable2.SetAttribute("Name", "$AI");
                    setVariable2.SetAttribute("Value", dynamicTrigger.AIAllowed.ToString());
                    initElement.AppendChild(setVariable2);

                    XmlElement setVariable3 = xml.CreateElement("SetVariable");
                    setVariable3.SetAttribute("Name", "$Node");
                    setVariable3.SetAttribute("Value", "COM");
                    initElement.AppendChild(setVariable3);

                    if (dynamicTrigger.PlaySound)
                    {
                        XmlElement setVariable4 = xml.CreateElement("SetVariable");
                        setVariable4.SetAttribute("Name", "Sound");
                        setVariable4.SetAttribute("Value", dynamicTrigger.Sound);
                        initElement.AppendChild(setVariable4);
                    }

                    XmlElement setVariable5 = xml.CreateElement("SetVariable");
                    setVariable5.SetAttribute("Name", "Flag1");
                    setVariable5.SetAttribute("Value", "0");
                    initElement.AppendChild(setVariable5);

                    // Create Trigger Content element
                    XmlElement triggerContentElement = xml.CreateElement("Content");

                    // Append Init to Content
                    triggerContentElement.AppendChild(initElement);

                    // Create Loop element
                    XmlElement loopElement = xml.CreateElement("Loop");

                    // Create Events element and EventBlock element
                    XmlElement eventsElement = xml.CreateElement("Events");
                    XmlElement eventBlockElement = xml.CreateElement("EventBlock");
                    eventBlockElement.SetAttribute("Template", "FreqUsed.Enter");
                    eventsElement.AppendChild(eventBlockElement);

                    // Append Events to Loop
                    loopElement.AppendChild(eventsElement);

                    // Create Actions element and ActionBlock element
                    XmlElement actionsElement = xml.CreateElement("Actions");
                    XmlElement actionBlockElement = xml.CreateElement("ActionBlock");
                    actionBlockElement.SetAttribute("Template", "FreqUsed.SwitchOff");
                    actionsElement.AppendChild(actionBlockElement);

                    // Create Transform element and append to Loop
                    XmlElement transformElement = xml.CreateElement("Transform");
                    transformElement.SetAttribute("Name", dynamicTrigger.TriggerTransformName);
                    actionsElement.AppendChild(transformElement);

                    if (dynamicTrigger.PlaySound)
                    {
                        // Create Actionsblock sound
                        XmlElement actionBlockSoundElement = xml.CreateElement("ActionBlock");
                        actionBlockSoundElement.SetAttribute("Template", "CommonLib.Sound");
                        actionsElement.AppendChild(actionBlockSoundElement);
                    }

                    // Append Actions to Loop
                    loopElement.AppendChild(actionsElement);

                    // Append Loop to Trigger
                    triggerContentElement.AppendChild(loopElement);

                    // Append Content to Trigger
                    T_element.AppendChild(triggerContentElement);

                    // Append Trigger to Content
                    contentElement.AppendChild(T_element);

                }
                else if (dynamicTrigger == null)
                {
                    if (childObject.name != "Camera")
                    {
                        if (childObject.GetComponent<TriggerSettings>() != null) //Checks if the trigger has a setting component
                        {
                            XmlElement T_element = xml.CreateElement("Trigger"); //Create a new node from scratch
                            TriggerSettings triggerSettings = childObject.GetComponent<TriggerSettings>(); //Trigger Settings.cs
                            T_element.SetAttribute("Name", Regex.Replace(childObject.name, @" \((.*?)\)", string.Empty)); //Add an name
                            T_element.SetAttribute("X", (childObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                            T_element.SetAttribute("Y", (-childObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

                            SpriteRenderer spriteRenderer = childObject.GetComponent<SpriteRenderer>();
                            if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
                            {

                                Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                                Vector3 scale = childObject.transform.localScale; // Get the GameObject scale

                                // Retrieve the image resolution of the sprite
                                float width = bounds.size.x * 100;
                                float height = bounds.size.y * 100;

                                // Set the width and height accordingly to the scale in the editor
                                T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                                T_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image

                                // Create the content node and add it to the trigger node
                                XmlElement cElement = xml.CreateElement("Content");

                                //xml doesn't format correctly so we load them into a separate doc
                                XmlDocument tempDoc = new XmlDocument();
                                tempDoc.LoadXml("<Content>" + triggerSettings.Content + "</Content>");
                                foreach (XmlNode childNode in tempDoc.DocumentElement.ChildNodes)
                                {
                                    XmlNode importedNode = xml.ImportNode(childNode, true);
                                    cElement.AppendChild(importedNode);
                                }

                                T_element.AppendChild(cElement);

                            }
                            contentElement.AppendChild(T_element);
                        }
                        else //continues as normal without any setting attached
                        {
                            XmlElement T_element = xml.CreateElement("Trigger"); //Create a new node from scratch
                            T_element.SetAttribute("Name", Regex.Replace(childObject.name, @" \((.*?)\)", string.Empty)); //Add an name
                            T_element.SetAttribute("X", (childObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                            T_element.SetAttribute("Y", (-childObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

                            SpriteRenderer spriteRenderer = childObject.GetComponent<SpriteRenderer>();
                            if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
                            {

                                Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                                Vector3 scale = childObject.transform.localScale; // Get the GameObject scale

                                // Retrieve the image resolution of the sprite
                                float width = bounds.size.x * 100;
                                float height = bounds.size.y * 100;

                                // Set the width and height accordingly to the scale in the editor
                                T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                                T_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image
                            }
                            contentElement.AppendChild(T_element);
                        }
                    }

                    //apply the modification to the build-map.xml with proper format
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "  ",
                        NewLineChars = "\r\n",
                        NewLineHandling = NewLineHandling.Replace
                    };

                    using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml", settings))
                    {
                        xml.Save(writer);
                    }
                }

            }

            // Add content to the object
            objectElement.AppendChild(contentElement);
        }

        node.AppendChild(objectElement); //Place it into the Object node
        xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
    }

}