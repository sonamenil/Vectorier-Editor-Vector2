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
using UnityEngine.UIElements;
using System.Collections.Generic;
using TreeEditor;


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
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }


                XmlElement imageNode;
                buildMap.ConvertToImage(node, xml, imageInScene, out imageNode, false);
                node.AppendChild(imageNode); //Place it into the Object node
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
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
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }

                XmlElement objectNode;
                buildMap.ConvertToObject(node, xml, objectInScene, out objectNode, false);
                node.AppendChild(objectNode);
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
            }

            foreach (GameObject spawnInScene in GameObject.FindGameObjectsWithTag("Spawn"))
            {
                UnityEngine.Transform parent = spawnInScene.transform.parent;
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Object" skip this GameObject and continue.
                    continue;
                }
                XmlElement spawnNode;
                buildMap.ConvertToSpawn(node, xml, spawnInScene, out spawnNode, false);
                node.AppendChild(spawnNode);
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
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
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                XmlElement animNode;
                buildMap.ConvertToAnimation(node, xml, animationInScene, out animNode, false);
                node.AppendChild(animNode);
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
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
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                XmlElement platformNode;
                buildMap.ConvertToPlatform(node, xml, platformInScene, out platformNode, false);
                node.AppendChild(platformNode);
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
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
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                XmlElement trapezoidNode;
                buildMap.ConvertToTrapezoid(node, xml, trapezoidInScene, out trapezoidNode, false);
                node.AppendChild(trapezoidNode);
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
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
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                XmlElement triggerNode;
                buildMap.ConvertToTrigger(node, xml, triggerInScene, out triggerNode, false);
                node.AppendChild(triggerNode);

                //apply the modification to the build-map.xml with proper format
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };

                using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml", settings))
                {
                    xml.Save(writer);
                }
            }

            foreach (GameObject triggerInScene in GameObject.FindGameObjectsWithTag("Dynamic Trigger"))
            {
                UnityEngine.Transform parent = triggerInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                XmlElement triggerNode;
                buildMap.ConvertToDynamicTrigger(node, xml, triggerInScene, out triggerNode, false);
                node.AppendChild(triggerNode);

                //apply the modification to the build-map.xml with proper format
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };

                using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml", settings))
                {
                    xml.Save(writer);
                }
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
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                XmlElement areaNode;
                buildMap.ConvertToArea(node, xml, areaInScene, out areaNode, false);
                node.AppendChild(areaNode);
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
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
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                XmlElement inNode;
                buildMap.ConvertToIn(node, xml, InTagInScene, out inNode, false);
                node.AppendChild(inNode);
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
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
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                XmlElement outNode;
                buildMap.ConvertToOut(node, xml, OutTagInScene, out outNode, false);
                node.AppendChild(outNode);
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
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
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                XmlElement objRefNode;
                buildMap.ConvertToObjectReference(node, xml, ObjectReferenceTagInScene, out objRefNode, false);
                node.AppendChild(objRefNode);
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
            }

            // Camera
            foreach (GameObject camInScene in GameObject.FindGameObjectsWithTag("Camera"))
            {
                //Note: This is actually a trigger, but with camera zoom properties
                UnityEngine.Transform parent = camInScene.transform.parent;
                if (parent != null && parent.CompareTag("Dynamic"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                if (parent != null && parent.CompareTag("Object"))
                {
                    // If the parent has the tag "Dynamic" skip this GameObject and continue.
                    continue;
                }
                XmlElement camNode;
                buildMap.ConvertToCamera(node, xml, camInScene, out camNode, false);
                node.AppendChild(camNode);
                xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + buildMap.mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
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


    void ConvertToImage(XmlNode node, XmlDocument xml, GameObject imageInScene, out XmlElement imageNode, bool localPosition)
    {
        imageNode = null;
        //Debug in log every images it write
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(imageInScene.name, @" \((.*?)\)", string.Empty));

        if (imageInScene.name != "Camera")
        {
            XmlElement ielement = xml.CreateElement("Image"); //Create a new node from scratch
            if (localPosition)
            {
                ielement.SetAttribute("X", Math.Round(imageInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                ielement.SetAttribute("Y", Math.Round(-imageInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                ielement.SetAttribute("X", Math.Round(imageInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                ielement.SetAttribute("Y", Math.Round(-imageInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
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

            imageNode = ielement;
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
        A = (float)Math.Round(cosZ * width * flipX);
        B = (-sinZ * width * flipX);
        C = (sinZ * height * flipY);
        D = (float)Math.Round(cosZ * height * flipY);

        // Tx and Ty are 0 if no rotation
        Tx = 0;
        Ty = 0;
    }


    void ConvertToAnimation(XmlNode node, XmlDocument xml, GameObject animationInScene, out XmlElement animNode, bool localPosition)
    {
        animNode = null;
        AnimationProperties AnimationComponent = animationInScene.GetComponent<AnimationProperties>(); // Animation Properties Component

        if (animationInScene.name != "Camera")
        {
            XmlElement animationElement = xml.CreateElement("Animation"); //Create a new node from scratch
            if (localPosition)
            {
                animationElement.SetAttribute("X", Math.Round(animationInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                animationElement.SetAttribute("Y", Math.Round(-animationInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                animationElement.SetAttribute("X", Math.Round(animationInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                animationElement.SetAttribute("Y", Math.Round(-animationInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
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
            animNode = animationElement;
        }
    }

    void ConvertToObjectReference(XmlNode node, XmlDocument xml, GameObject ObjectReferenceTagInScene, out XmlElement objRefNode, bool localPosition)
    {
        objRefNode = null;
        if (ObjectReferenceTagInScene.name != "Camera")
        {

            XmlElement RefElement = xml.CreateElement("ObjectReference"); //Create a new node from scratch
            RefElement.SetAttribute("Name", Regex.Replace(ObjectReferenceTagInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
            if (localPosition)
            {
                RefElement.SetAttribute("X", Math.Round(ObjectReferenceTagInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                RefElement.SetAttribute("Y", Math.Round(-ObjectReferenceTagInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                RefElement.SetAttribute("X", Math.Round(ObjectReferenceTagInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                RefElement.SetAttribute("Y", Math.Round(-ObjectReferenceTagInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            XmlElement propertiesElement = xml.CreateElement("Properties");
            XmlElement staticElement = xml.CreateElement("Static");

            if (ObjectReferenceTagInScene.GetComponent<ObjectReference>())
            {
                ObjectReference objectReference = ObjectReferenceTagInScene.GetComponent<ObjectReference>();

                RefElement.SetAttribute("Filename", objectReference.FileName.ToString() + ".xml"); //Add an name

                if (objectReference.useCustomVariables)
                {
                    XmlElement contentElement = xml.CreateElement("Properties");

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
            }

            else if (ObjectReferenceTagInScene.GetComponent<Panels>())
            {
                Panels panels = ObjectReferenceTagInScene.GetComponent<Panels>();
                SpriteRenderer spriteRenderer = panels.spriteRenderer;
                Bounds bounds = spriteRenderer.sprite.bounds;

                XmlElement overrideVariable = xml.CreateElement("OverrideVariable");
                XmlElement variables = xml.CreateElement("Variable");
                XmlElement variables1 = xml.CreateElement("Variable");

                variables.SetAttribute("Name", "Width");
                variables.SetAttribute("Value", (bounds.size.x * 100 * spriteRenderer.transform.localScale.x + 10).ToString());

                variables1.SetAttribute("Name", "Height");
                variables1.SetAttribute("Value", (bounds.size.y * 100 * spriteRenderer.transform.localScale.y + 10).ToString());

                if (panels.useCustomVariables)
                {
                    XmlDocument tempDoc = new XmlDocument();
                    tempDoc.LoadXml("<OverrideVariable>" + panels.CustomVariables + "</OverrideVariable>");
                    foreach (XmlNode childNode in tempDoc.DocumentElement.ChildNodes)
                    {
                        XmlNode importedNode = xml.ImportNode(childNode, true);
                        overrideVariable.AppendChild(importedNode);
                    }
                }

                overrideVariable.AppendChild(variables);
                overrideVariable.AppendChild(variables1);
                staticElement.AppendChild(overrideVariable);
                propertiesElement.AppendChild(staticElement);
                RefElement.SetAttribute("Filename", "wall_props.xml"); //Add an name
                RefElement.AppendChild(propertiesElement);

            }

            else if (ObjectReferenceTagInScene.GetComponent<Laser>())
            {
                Laser laser = ObjectReferenceTagInScene.GetComponent<Laser>();
                SpriteRenderer spriteRenderer = laser.laser;
                GameObject actitrigger = laser.laserActivatorTrigger;
                Bounds bounds = spriteRenderer.sprite.bounds;

                XmlElement overrideVariable = xml.CreateElement("OverrideVariable");
                XmlElement variables = xml.CreateElement("Variable");
                XmlElement flame = xml.CreateElement("Variable");
                XmlElement tesla = xml.CreateElement("Variable");
                XmlElement alllaser = xml.CreateElement("Variable");
                XmlElement activatorY = xml.CreateElement("Variable");
                XmlElement activatorX = xml.CreateElement("Variable");
                XmlElement globalTimer = xml.CreateElement("Variable");

                variables.SetAttribute("Name", "LaserReachDistance");
                variables.SetAttribute("Value", (bounds.size.y * 100 * spriteRenderer.transform.localScale.y).ToString());

                flame.SetAttribute("Name", "AllowFlame");
                flame.SetAttribute("Value", "0");

                tesla.SetAttribute("Name", "AllowTesla");
                tesla.SetAttribute("Value", "0");

                alllaser.SetAttribute("Name", "AllowLaser");
                alllaser.SetAttribute("Value", "0");

                activatorX.SetAttribute("Name", "TriggerActivatorX");
                activatorX.SetAttribute("Value", (actitrigger.transform.localPosition.x * 100).ToString());

                activatorY.SetAttribute("Name", "TriggerActivatorY");
                activatorY.SetAttribute("Value", (-actitrigger.transform.localPosition.y * 100).ToString());

                globalTimer.SetAttribute("Name", "GlobalTimer");
                globalTimer.SetAttribute("Value", (laser.globalTimer * 60).ToString());

                overrideVariable.AppendChild(variables);
                if (laser.allowFlame != true)
                {
                    overrideVariable.AppendChild(flame);
                }
                if (laser.allowLaser != true)
                {
                    overrideVariable.AppendChild(alllaser);
                }
                if (laser.allowTesla != true)
                {
                    overrideVariable.AppendChild(tesla);
                }

                overrideVariable.AppendChild(activatorX);
                overrideVariable.AppendChild(activatorY);

                if (laser.useGlobalTimer != false)
                {
                    overrideVariable.AppendChild(globalTimer);
                }
                staticElement.AppendChild(overrideVariable);
                propertiesElement.AppendChild(staticElement);
                RefElement.SetAttribute("Filename", "traps.xml"); //Add an name
                RefElement.AppendChild(propertiesElement);

            }

            else if (ObjectReferenceTagInScene.GetComponent<BlackBall>())
            {
                BlackBall ball = ObjectReferenceTagInScene.GetComponent<BlackBall>();
                GameObject actitrigger = ball.ballActivatorTrigger;
                SpriteRenderer sprite = actitrigger.GetComponent<SpriteRenderer>();

                XmlElement overrideVariable = xml.CreateElement("OverrideVariable");
                XmlElement type = xml.CreateElement("Variable");
                XmlElement activatorY = xml.CreateElement("Variable");
                XmlElement activatorX = xml.CreateElement("Variable");
                XmlElement activatorH = xml.CreateElement("Variable");
                XmlElement globalTimer = xml.CreateElement("Variable");
                XmlElement enableArea = xml.CreateElement("Variable");
                XmlElement toFly = xml.CreateElement("Variable");

                type.SetAttribute("Name", "Type");
                type.SetAttribute("Value", ball.type.ToString());

                activatorX.SetAttribute("Name", "TriggerActivatorX");
                activatorX.SetAttribute("Value", (actitrigger.transform.localPosition.x * 100).ToString());

                activatorY.SetAttribute("Name", "TriggerActivatorY");
                activatorY.SetAttribute("Value", (-actitrigger.transform.localPosition.y * 100).ToString());

                activatorH.SetAttribute("Name", "TriggerActivatorH");
                activatorH.SetAttribute("Value", (sprite.sprite.bounds.size.y * 100 * actitrigger.transform.localScale.y).ToString());

                globalTimer.SetAttribute("Name", "GlobalTimer");
                globalTimer.SetAttribute("Value", (ball.globalTimer * 60).ToString());

                enableArea.SetAttribute("Name", "EnableArea");
                enableArea.SetAttribute("Value", Convert.ToInt32(ball.enableArea).ToString());

                toFly.SetAttribute("Name", "ToFly");
                toFly.SetAttribute("Value", Convert.ToInt32(ball.toFly).ToString());

                if (ball.useType == true)
                {
                    overrideVariable.AppendChild(type);
                }
                overrideVariable.AppendChild(activatorX);
                overrideVariable.AppendChild(activatorY);
                overrideVariable.AppendChild(activatorH);
                overrideVariable.AppendChild(enableArea);
                if (ball.enableArea == true) 
                {
                    overrideVariable.AppendChild(toFly);
                }


                if (ball.useGlobalTimer != false)
                {
                    overrideVariable.AppendChild(globalTimer);
                }
                staticElement.AppendChild(overrideVariable);
                propertiesElement.AppendChild(staticElement);
                RefElement.SetAttribute("Filename", "traps.xml"); //Add an name
                RefElement.AppendChild(propertiesElement);

            }
            Vector3 scale = ObjectReferenceTagInScene.transform.localScale;

            if (Mathf.Abs(ObjectReferenceTagInScene.transform.eulerAngles.z) > Mathf.Epsilon || scale.x != 1 || scale.y != 1)
            {
                if (ObjectReferenceTagInScene.GetComponent<Panels>())
                {
                    float A, B, C, D, Tx, Ty;
                    ConvertToMarmaladeMatrix(ObjectReferenceTagInScene, 1, 1, out A, out B, out C, out D, out Tx, out Ty);

                    XmlElement matrixElement = xml.CreateElement("Matrix");
                    matrixElement.SetAttribute("A", A.ToString());
                    matrixElement.SetAttribute("B", B.ToString());
                    matrixElement.SetAttribute("C", C.ToString());
                    matrixElement.SetAttribute("D", D.ToString());
                    matrixElement.SetAttribute("Tx", Tx.ToString());
                    matrixElement.SetAttribute("Ty", Ty.ToString());

                    staticElement.AppendChild(matrixElement);
                    propertiesElement.AppendChild(staticElement);
                    RefElement.AppendChild(propertiesElement);
                }
                else
                {
                    float A, B, C, D, Tx, Ty;
                    ConvertToMarmaladeMatrix(ObjectReferenceTagInScene, scale.x, scale.y, out A, out B, out C, out D, out Tx, out Ty);

                    XmlElement matrixElement = xml.CreateElement("Matrix");
                    matrixElement.SetAttribute("A", A.ToString());
                    matrixElement.SetAttribute("B", B.ToString());
                    matrixElement.SetAttribute("C", C.ToString());
                    matrixElement.SetAttribute("D", D.ToString());
                    matrixElement.SetAttribute("Tx", Tx.ToString());
                    matrixElement.SetAttribute("Ty", Ty.ToString());

                    staticElement.AppendChild(matrixElement);
                    propertiesElement.AppendChild(staticElement);
                    RefElement.AppendChild(propertiesElement);
                }
                // Convert the rotation to the Marmalade transformation matrix
                


            }

            objRefNode = RefElement;
        }


    }




    void ConvertToPlatform(XmlNode node, XmlDocument xml, GameObject platformInScene, out XmlElement platformNode, bool localPosition) // Platform Collision (Invisible block that is collide-able)
    {
        platformNode = null;
        //Debug in log every platform it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(platformInScene.name, @" \((.*?)\)", string.Empty));

        if (platformInScene.name != "Camera")
        {
            XmlElement P_element = xml.CreateElement("Platform"); //Create a new node from scratch
            if (localPosition)
            {
                P_element.SetAttribute("X", Math.Round(platformInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                P_element.SetAttribute("Y", Math.Round(-platformInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                P_element.SetAttribute("X", Math.Round(platformInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                P_element.SetAttribute("Y", Math.Round(-platformInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }

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
            platformNode = P_element;
        }

    }

    void ConvertToSpawn(XmlNode node, XmlDocument xml, GameObject spawnInScene, out XmlElement spawnNode, bool localPosition)
    {
        spawnNode = null;

        Respawn RespawnComponent = spawnInScene.GetComponent<Respawn>(); //Respawn component
        Spawn Spawn = spawnInScene.GetComponent<Spawn>(); //spawn component
        XmlElement spawnElement = xml.CreateElement("Spawn");
        Spawn[] SpawnComponent = FindObjectsOfType<Spawn>();


        if (RespawnComponent != null)
        {
            // Root
            XmlElement objectElement = xml.CreateElement("Object");
            objectElement.SetAttribute("X", "0");
            objectElement.SetAttribute("Y", "0");

            // Content
            XmlElement contentElement = xml.CreateElement("Content");


            foreach (Spawn spawns in SpawnComponent)
            {
                GameObject gameObjwithSpawnComponent = spawns.gameObject; //check every game object that has the spawn component
                if (RespawnComponent.RespawnName == gameObjwithSpawnComponent.GetComponent<Spawn>().SpawnName)
                {
                    if (gameObjwithSpawnComponent.GetComponent<Spawn>().RefersToRespawn)
                    {
                        // spawn element
                        XmlElement spawnInsideElement = xml.CreateElement("Spawn");
                        if (localPosition)
                        {
                            spawnInsideElement.SetAttribute("X", Math.Round(gameObjwithSpawnComponent.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                            spawnInsideElement.SetAttribute("Y", Math.Round(-gameObjwithSpawnComponent.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                        }
                        else
                        {
                            spawnInsideElement.SetAttribute("X", Math.Round(gameObjwithSpawnComponent.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                            spawnInsideElement.SetAttribute("Y", Math.Round(-gameObjwithSpawnComponent.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                        }
                        spawnInsideElement.SetAttribute("Name", gameObjwithSpawnComponent.GetComponent<Spawn>().SpawnName);
                        spawnInsideElement.SetAttribute("Animation", gameObjwithSpawnComponent.GetComponent<Spawn>().SpawnAnimation);
                        contentElement.AppendChild(spawnInsideElement);
                    }
                }
            }

            //Trigger element
            XmlElement triggerElement = xml.CreateElement("Trigger");
            triggerElement.SetAttribute("Name", RespawnComponent.TriggerName);
            if (localPosition)
            {
                triggerElement.SetAttribute("X", Math.Round(spawnInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                triggerElement.SetAttribute("Y", Math.Round(-spawnInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                triggerElement.SetAttribute("X", Math.Round(spawnInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                triggerElement.SetAttribute("Y", Math.Round(-spawnInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }


            SpriteRenderer spriteRenderer = spawnInScene.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
            {

                Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                Vector3 scale = spawnInScene.transform.localScale; // Get the GameObject scale

                // Retrieve the image resolution of the sprite
                float width = bounds.size.x * 100;
                float height = bounds.size.y * 100;

                // Set the width and height accordingly to the scale in the editor
                triggerElement.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                triggerElement.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image
            }

            // Create the properties element and its child static element
            XmlElement propertiesElement = xml.CreateElement("Properties");
            XmlElement staticElement = xml.CreateElement("Static");
            XmlElement selectionElement = xml.CreateElement("Selection");
            selectionElement.SetAttribute("Choice", "AITriggers");
            selectionElement.SetAttribute("Variant", "CommonMode");

            staticElement.AppendChild(selectionElement);
            propertiesElement.AppendChild(staticElement);
            triggerElement.AppendChild(propertiesElement);

            XmlElement triggerContentElement = xml.CreateElement("Content"); // create content element inside trigger element
            XmlElement initElement = xml.CreateElement("Init"); // create the init element and its child setVariable element

            float Frames = RespawnComponent.RespawnSecond * 60;

            string[][] setVariables =
            {
                new[] { "Name", "$Active", "Value", "1" },
                new[] { "Name", "$Node", "Value", "COM" },
                new[] { "Name", "Spawn", "Value", RespawnComponent.RespawnName },
                new[] { "Name", "Frames", "Value", Frames.ToString() },
                new[] { "Name", "SpawnModel", "Value", RespawnComponent.Spawnmodel },
                new[] { "Name", "Reversed", "Value", "0" },
                new[] { "Name", "$AI", "Value", "0" },
                new[] { "Name", "Flag1", "Value", "0" },
            };

            // add each setVariable element to the init element
            foreach (var setVariable in setVariables)
            {
                XmlElement setVariableElement = xml.CreateElement("SetVariable");
                setVariableElement.SetAttribute(setVariable[0], setVariable[1]);
                setVariableElement.SetAttribute(setVariable[2], setVariable[3]);
                initElement.AppendChild(setVariableElement);
            }

            triggerContentElement.AppendChild(initElement);

            // create template element inside content element
            if (RespawnComponent.RespawnOnScreen)
            {
                XmlElement templateElement = xml.CreateElement("Loop");
                templateElement.SetAttribute("Template", "Respawn_OnScreen.Player");
                XmlElement templateElement2 = xml.CreateElement("Loop");
                templateElement2.SetAttribute("Template", "Respawn_OnScreen.Timeout");
                triggerContentElement.AppendChild(templateElement);
                triggerContentElement.AppendChild(templateElement2);
            }
            else
            {
                XmlElement templateElement = xml.CreateElement("Template");
                templateElement.SetAttribute("Name", "Respawn_OnScreen");
                triggerContentElement.AppendChild(templateElement);
            }


            triggerElement.AppendChild(triggerContentElement);
            contentElement.AppendChild(triggerElement);
            objectElement.AppendChild(contentElement);

            spawnNode = objectElement;

        }
        else if (RespawnComponent == null && Spawn != null)
        {
            if (Spawn.RefersToRespawn == false)
            {
                if (localPosition)
                {
                    spawnElement.SetAttribute("X", Math.Round(spawnInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    spawnElement.SetAttribute("Y", Math.Round(-spawnInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                }
                else
                {
                    spawnElement.SetAttribute("X", Math.Round(spawnInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    spawnElement.SetAttribute("Y", Math.Round(-spawnInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                }
                spawnElement.SetAttribute("Name", Spawn.SpawnName); // name in the spawn component
                spawnElement.SetAttribute("Animation", Spawn.SpawnAnimation); // spawnanim in spawn component

                spawnNode = spawnElement;
            }

        }

    }

    void ConvertToTrapezoid(XmlNode node, XmlDocument xml, GameObject trapezoidInScene, out XmlElement trapezoidNode, bool localPosition) // Trapezoid Collision (Slope)
    {
        trapezoidNode = null;
        //Debug in log every platform it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(trapezoidInScene.name, @" \((.*?)\)", string.Empty));

        if (Regex.Replace(trapezoidInScene.name, @" \((.*?)\)", string.Empty) == "trapezoid_type1") // Slope Default
        {
            XmlElement T_element = xml.CreateElement("Trapezoid"); //Create a new node from scratch
            if (localPosition)
            {
                T_element.SetAttribute("X", Math.Round(trapezoidInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                T_element.SetAttribute("Y", Math.Round(-trapezoidInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                T_element.SetAttribute("X", Math.Round(trapezoidInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                T_element.SetAttribute("Y", Math.Round(-trapezoidInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }

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

            trapezoidNode = T_element;
        }

        else if (Regex.Replace(trapezoidInScene.name, @" \((.*?)\)", string.Empty) == "trapezoid_type2") // Slope Mirrored
        {
            XmlElement T_element = xml.CreateElement("Trapezoid"); //Create a new node from scratch
            if (localPosition)
            {
                T_element.SetAttribute("X", Math.Round(trapezoidInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                T_element.SetAttribute("Y", Math.Round(-trapezoidInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                T_element.SetAttribute("X", Math.Round(trapezoidInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                T_element.SetAttribute("Y", Math.Round(-trapezoidInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }

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

            trapezoidNode = T_element;
        }
    }

    void ConvertToTrigger(XmlNode node, XmlDocument xml, GameObject triggerInScene, out XmlElement triggerNode, bool localPosition)
    {
        triggerNode = null;
        //Debug in log every trigger it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(triggerInScene.name, @" \((.*?)\)", string.Empty));

        if (triggerInScene.name != "Camera")
        {
            DynamicTrigger dynamicTrigger = triggerInScene.GetComponent<DynamicTrigger>();
            TriggerSettings triggerSettings = triggerInScene.GetComponent<TriggerSettings>(); //Trigger Settings.cs

            if (triggerSettings && dynamicTrigger)
            {
                Debug.LogError($"GameObject '{triggerInScene.name}' cannot contain both TriggerSetting and DynamicTrigger.");
                return;
            }
            else if (!triggerSettings && !dynamicTrigger)
            {
                Debug.LogError($"GameObject '{triggerInScene.name}' must contain at least TriggerSetting or DynamicTrigger.");
                return;
            }

            if (dynamicTrigger != null)
            {
                XmlElement T_element = xml.CreateElement("Trigger"); //Create a new node from scratch
                T_element.SetAttribute("Name", Regex.Replace(triggerInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
                if (localPosition)
                {
                    T_element.SetAttribute("X", Math.Round(triggerInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    T_element.SetAttribute("Y", Math.Round(-triggerInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                }
                else
                {
                    T_element.SetAttribute("X", Math.Round(triggerInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    T_element.SetAttribute("Y", Math.Round(-triggerInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                }

                SpriteRenderer spriteRenderer = triggerInScene.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Sprite Size in Width and Height
                {

                    Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                    Vector3 scale = triggerInScene.transform.localScale; // Get the GameObject scale

                    float width = bounds.size.x * 100;
                    float height = bounds.size.y * 100;

                    T_element.SetAttribute("Width", (width * scale.x).ToString()); //Width of the Image
                    T_element.SetAttribute("Height", (height * scale.y).ToString()); //Height of the Image
                }
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

                XmlElement triggerContentElement = xml.CreateElement("Content");
                triggerContentElement.AppendChild(initElement);

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

                if (dynamicTrigger.MultipleTransformation)
                {
                    XmlElement chooseElement = xml.CreateElement("Choose");
                    chooseElement.SetAttribute("Order", dynamicTrigger.Order.ToString());
                    chooseElement.SetAttribute("Set", dynamicTrigger.Set.ToString());

                    foreach (string transformationName in dynamicTrigger.TransformationNames)
                    {
                        XmlElement transformElement = xml.CreateElement("Transform");
                        transformElement.SetAttribute("Name", transformationName);
                        chooseElement.AppendChild(transformElement);
                    }

                    actionsElement.AppendChild(chooseElement);
                }
                else
                {
                    XmlElement transformElement = xml.CreateElement("Transform");
                    transformElement.SetAttribute("Name", dynamicTrigger.TriggerTransformName);
                    actionsElement.AppendChild(transformElement);
                }

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

                triggerNode = T_element;
            }

            if (triggerSettings != null) //Checks if the trigger has a setting component
            {
                XmlElement T_element = xml.CreateElement("Trigger"); //Create a new node from scratch
                T_element.SetAttribute("Name", Regex.Replace(triggerInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
                if (localPosition)
                {
                    T_element.SetAttribute("X", Math.Round(triggerInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    T_element.SetAttribute("Y", Math.Round(-triggerInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                }
                else
                {
                    T_element.SetAttribute("X", Math.Round(triggerInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                    T_element.SetAttribute("Y", Math.Round(-triggerInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
                }

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

                    triggerNode = T_element; //Place it into the Object node

                }
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


    void ConvertToDynamicTrigger(XmlNode node, XmlDocument xml, GameObject dynamictriggerInScene, out XmlElement dyntriggerNode, bool localPosition)
    {
        dyntriggerNode = null;
        DynamicTrigger dynamicTrigger = dynamictriggerInScene.GetComponent<DynamicTrigger>();
        XmlElement T_element = xml.CreateElement("Trigger");
        T_element.SetAttribute("Name", "");
        T_element.SetAttribute("Name", Regex.Replace(dynamictriggerInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
        if (localPosition)
        {
            T_element.SetAttribute("X", Math.Round(dynamictriggerInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            T_element.SetAttribute("Y", Math.Round(-dynamictriggerInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
        }
        else
        {
            T_element.SetAttribute("X", Math.Round(dynamictriggerInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            T_element.SetAttribute("Y", Math.Round(-dynamictriggerInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
        }
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
        dyntriggerNode = T_element;

    }

    void ConvertToArea(XmlNode node, XmlDocument xml, GameObject areaInScene, out XmlElement areaNode, bool localPosition)
    {
        areaNode = null;
        //Debug in log every Area it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(areaInScene.name, @" \((.*?)\)", string.Empty));

        if (areaInScene.name != "Camera")
        {
            XmlElement A_element = xml.CreateElement("Area"); //Create a new node from scratch
            A_element.SetAttribute("Name", Regex.Replace(areaInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
            if (localPosition)
            {
                A_element.SetAttribute("X", Math.Round(areaInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                A_element.SetAttribute("Y", Math.Round(-areaInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                A_element.SetAttribute("X", Math.Round(areaInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                A_element.SetAttribute("Y", Math.Round(-areaInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }

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
            areaNode = A_element;
        }
    }


    void ConvertToIn(XmlNode node, XmlDocument xml, GameObject InTagInScene, out XmlElement inNode, bool localPosition)
    {
        inNode = null;
        if (InTagInScene.name != "Camera")
        {
            XmlElement A_element = xml.CreateElement("In"); //Create a new node from scratch
            A_element.SetAttribute("Name", Regex.Replace(InTagInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
            if (localPosition)
            {
                A_element.SetAttribute("X", Math.Round(InTagInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                A_element.SetAttribute("Y", Math.Round(-InTagInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                A_element.SetAttribute("X", Math.Round(InTagInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                A_element.SetAttribute("Y", Math.Round(-InTagInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }


            inNode = A_element;
        }
    }

    void ConvertToOut(XmlNode node, XmlDocument xml, GameObject OutTagInScene, out XmlElement outNode, bool localPosition)
    {
        outNode = null;
        if (OutTagInScene.name != "Camera")
        {
            XmlElement A_element = xml.CreateElement("Out"); //Create a new node from scratch
            A_element.SetAttribute("Name", Regex.Replace(OutTagInScene.name, @" \((.*?)\)", string.Empty)); //Add an name
            if (localPosition)
            {
                A_element.SetAttribute("X", Math.Round(OutTagInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                A_element.SetAttribute("Y", Math.Round(-OutTagInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                A_element.SetAttribute("X", Math.Round(OutTagInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                A_element.SetAttribute("Y", Math.Round(-OutTagInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }


            outNode = A_element;
        }
    }

    void ConvertToCamera(XmlNode node, XmlDocument xml, GameObject camInScene, out XmlElement cameraNode, bool localPosition)
    {
        cameraNode = null;

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
            if (localPosition)
            {
                triggerElement.SetAttribute("X", Math.Round(camInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                triggerElement.SetAttribute("Y", Math.Round(-camInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            else
            {
                triggerElement.SetAttribute("X", Math.Round(camInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                triggerElement.SetAttribute("Y", Math.Round(-camInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
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
            cameraNode = triggerElement;
        }
    }

    void ConvertToDynamic(XmlNode node, XmlDocument xml, GameObject dynamicInScene, UnityEngine.Transform dynamicInSceneTransform)
    {

        // Get all Dynamic components
        Dynamic[] dynamicComponents = dynamicInScene.GetComponents<Dynamic>();
        DynamicColor dynamicColorParent = dynamicInScene.GetComponent<DynamicColor>();

        // Object
        XmlElement objectElement = xml.CreateElement("Object");
        objectElement.SetAttribute("X", "0");
        objectElement.SetAttribute("Y", "0");

        // Properties
        XmlElement propertiesElement = xml.CreateElement("Properties");

        // Dynamic
        XmlElement dynamicElement = xml.CreateElement("Dynamic");

        foreach (var dynamicComponent in dynamicComponents)
        {
            XmlElement transformationElement = xml.CreateElement("Transformation");
            transformationElement.SetAttribute("Name", dynamicComponent.TransformationName);


            // Handle Move Intervals (1 to 5)
            for (int i = 1; i <= 5; i++)
            {
                var movementUsage = dynamicComponent.MovementUsage;
                var moveInterval = GetMoveInterval(dynamicComponent, i); // Get MoveInterval by index

                if (movementUsage != null && movementUsage.UseMovement(i) && moveInterval != null)
                {
                    XmlElement moveIntervalElement = xml.CreateElement("MoveInterval");

                    int framesToMove = Mathf.Max(1, Mathf.RoundToInt(moveInterval.MoveDuration * 60));
                    int delayFrames = Mathf.RoundToInt(moveInterval.Delay * 60);
                    moveIntervalElement.SetAttribute("Frames", framesToMove.ToString());
                    moveIntervalElement.SetAttribute("Type", "Bezier");

                    // Create Points (Start, Support, Finish)
                    XmlElement startPointElement = CreatePointElement(xml, 0, 0);
                    XmlElement supportPointElement = CreatePointElement(xml, moveInterval.SupportXAxis * 100, -moveInterval.SupportYAxis * 100);

                    XmlElement finishPointElement = CreatePointElement(xml, moveInterval.MoveXAxis * 100, -moveInterval.MoveYAxis * 100);

                    XmlElement delayInterval = xml.CreateElement("DelayInterval");
                    delayInterval.SetAttribute("Frames", delayFrames.ToString());

                    // Append points to MoveInterval
                    moveIntervalElement.AppendChild(startPointElement);
                    moveIntervalElement.AppendChild(supportPointElement);
                    moveIntervalElement.AppendChild(finishPointElement);

                    if (delayFrames != 0)
                    {
                        transformationElement.AppendChild(delayInterval);
                    }
                    transformationElement.AppendChild(moveIntervalElement);


                }
            }
            dynamicElement.AppendChild(transformationElement);
        }

        // DynamicColor 
        if (dynamicColorParent != null)
        {
            XmlElement transformationElement = xml.CreateElement("Transformation");
            transformationElement.SetAttribute("Name", dynamicColorParent.TransformationName);

            XmlElement colorElement = xml.CreateElement("Color");

            // Set ColorStart (StartColor) and ColorFinish (EndColor)
            string startColorHex = ColorUtility.ToHtmlStringRGB(dynamicColorParent.StartColor) + Mathf.RoundToInt(dynamicColorParent.StartColor.a * 255).ToString("X2");
            string finishColorHex = ColorUtility.ToHtmlStringRGB(dynamicColorParent.EndColor) + Mathf.RoundToInt(dynamicColorParent.EndColor.a * 255).ToString("X2");

            colorElement.SetAttribute("ColorStart", $"#{startColorHex}");
            colorElement.SetAttribute("ColorFinish", $"#{finishColorHex}");

            // Calculate Frames (Duration * 60) or 1 if Duration is 0
            int frames = dynamicColorParent.Duration > 0 ? Mathf.CeilToInt(dynamicColorParent.Duration * 60) : 1;
            colorElement.SetAttribute("Frames", frames.ToString());

            transformationElement.AppendChild(colorElement);
            dynamicElement.AppendChild(transformationElement);
        }



        propertiesElement.AppendChild(dynamicElement);
        objectElement.AppendChild(propertiesElement);

        // Create Content element
        XmlElement contentElement = xml.CreateElement("Content");

        // image list for the dynamic
        List<GameObject> ImageObjects = new List<GameObject>();

        // add image to the list
        foreach (UnityEngine.Transform child in dynamicInSceneTransform)
        {
            if (child.gameObject.CompareTag("Image"))
            {
                ImageObjects.Add(child.gameObject);
            }
        }

        // sort the list based on order in layer
        ImageObjects.Sort((a, b) =>
        {
            SpriteRenderer rendererA = a.GetComponent<SpriteRenderer>();
            SpriteRenderer rendererB = b.GetComponent<SpriteRenderer>();

            // Handle cases where SpriteRenderer might be null
            int orderA = rendererA != null ? rendererA.sortingOrder : 0;
            int orderB = rendererB != null ? rendererB.sortingOrder : 0;

            return orderA.CompareTo(orderB);
        });

        foreach (GameObject imageObject in ImageObjects)
        {
            XmlElement ielement = xml.CreateElement("Image"); //Create a new node from scratch
            ielement.SetAttribute("X", (imageObject.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            ielement.SetAttribute("Y", (-imageObject.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            ielement.SetAttribute("ClassName", Regex.Replace(imageObject.name, @" \((.*?)\)", string.Empty)); //Add a name
            SpriteRenderer spriteRenderer = imageObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) //Get the Image Size in Width and Height
            {

                Bounds bounds = spriteRenderer.sprite.bounds;// Get the bounds of the sprite
                Vector3 scale = imageObject.transform.localScale; // Get the GameObject scale
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
                    ConvertToMarmaladeMatrix(imageObject, width * scale.x, height * scale.y, out A, out B, out C, out D, out Tx, out Ty);

                    XmlElement matrixElement = xml.CreateElement("Matrix");
                    matrixElement.SetAttribute("A", A.ToString());
                    matrixElement.SetAttribute("B", B.ToString());
                    matrixElement.SetAttribute("C", C.ToString());
                    matrixElement.SetAttribute("D", D.ToString());
                    matrixElement.SetAttribute("Tx", Tx.ToString());
                    matrixElement.SetAttribute("Ty", Ty.ToString());

                    XmlElement imgpropertiesElement = xml.CreateElement("Properties");
                    XmlElement staticElement = xml.CreateElement("Static");
                    staticElement.AppendChild(matrixElement);
                    imgpropertiesElement.AppendChild(staticElement);
                    ielement.AppendChild(imgpropertiesElement);

                    Color color = spriteRenderer.color;
                    if (color.r != 1.000 || color.g != 1.000 || color.b != 1.000)
                    {
                        XmlElement colorElement = xml.CreateElement("StartColor");
                        colorElement.SetAttribute("Color", "#" + ColorUtility.ToHtmlStringRGB(color).ToString());
                        staticElement.AppendChild(colorElement);
                    }

                    if (Regex.Replace(imageObject.name, @" \((.*?)\)", string.Empty) == "traps_shadows.gradient")
                    {
                        XmlElement blendElement = xml.CreateElement("BlendMode");
                        blendElement.SetAttribute("Mode", "Multiply");
                        staticElement.AppendChild(blendElement);
                    }
                    if (Regex.Replace(imageObject.name, @" \((.*?)\)", string.Empty) == "traps_shadows.gradient_rounded")
                    {
                        XmlElement blendElement = xml.CreateElement("BlendMode");
                        blendElement.SetAttribute("Mode", "Multiply");
                        staticElement.AppendChild(blendElement);
                    }
                    if (Regex.Replace(imageObject.name, @" \((.*?)\)", string.Empty) == "traps_shadows.gradient_intense")
                    {
                        XmlElement blendElement = xml.CreateElement("BlendMode");
                        blendElement.SetAttribute("Mode", "Multiply");
                        staticElement.AppendChild(blendElement);
                    }
                    if (Regex.Replace(imageObject.name, @" \((.*?)\)", string.Empty) == "traps_shadows.gradient_intense_rounded")
                    {
                        XmlElement blendElement = xml.CreateElement("BlendMode");
                        blendElement.SetAttribute("Mode", "Multiply");
                        staticElement.AppendChild(blendElement);
                    }

                }
            }
            contentElement.AppendChild(ielement);
        }

        foreach (UnityEngine.Transform childObject in dynamicInSceneTransform)
        {
            //check if the gameobject has specific tag

            if (childObject.gameObject.CompareTag("Object"))
            {

                XmlElement objectNode;
                ConvertToObject(node, xml, childObject.gameObject, out objectNode, false);
                contentElement.AppendChild(objectNode);

            }

            else if (childObject.gameObject.CompareTag("Platform"))
            {
                //Platform

                XmlElement platformNode;
                ConvertToPlatform(node, xml, childObject.gameObject, out platformNode, false);
                contentElement.AppendChild(platformNode);

            }

            else if (childObject.gameObject.CompareTag("Trapezoid"))
            {
                // Trapezoid
                XmlElement trapezoidNode;
                ConvertToTrapezoid(node, xml, childObject.gameObject, out trapezoidNode, false);
                contentElement.AppendChild(trapezoidNode);

            }
            else if (childObject.gameObject.CompareTag("Area"))
            {
                XmlElement areaNode;
                ConvertToArea(node, xml, childObject.gameObject, out areaNode, false);
                contentElement.AppendChild(areaNode);
            }

            else if (childObject.gameObject.CompareTag("Trigger"))
            {
                XmlElement triggerNode;
                ConvertToTrigger(node, xml, childObject.gameObject, out triggerNode, false);
                contentElement.AppendChild(triggerNode);

            }

            else if (childObject.gameObject.CompareTag("Dynamic Trigger"))
            {
                XmlElement triggerNode;
                ConvertToDynamicTrigger(node, xml, childObject.gameObject, out triggerNode, false);
                contentElement.AppendChild(triggerNode);

            }

            else if (childObject.gameObject.CompareTag("Object Reference"))
            {

                XmlElement modelNode;
                ConvertToObjectReference(node, xml, childObject.gameObject, out modelNode, false);
                contentElement.AppendChild(modelNode);
            }

            else if (childObject.gameObject.CompareTag("Animation"))
            {
                XmlElement animNode;
                ConvertToAnimation(node, xml, childObject.gameObject, out animNode, false);
                contentElement.AppendChild(animNode);
            }
            else if (childObject.gameObject.CompareTag("Camera"))
            {
                XmlElement camNode;
                ConvertToCamera(node, xml, childObject.gameObject, out camNode, false);
                contentElement.AppendChild(camNode);
            }

            // Add content to the object
            objectElement.AppendChild(contentElement);
        }

        XmlElement CreatePointElement(XmlDocument xml, float x, float y)
        {
            XmlElement pointElement = xml.CreateElement("Point");
            pointElement.SetAttribute("X", x.ToString("0.0"));
            pointElement.SetAttribute("Y", y.ToString("0.0"));
            return pointElement;
        }

        // Helper Method: Get MoveInterval by index
        Dynamic.Movement GetMoveInterval(Dynamic dynamicComponent, int index)
        {
            switch (index)
            {
                case 1: return dynamicComponent.MoveInterval1;
                case 2: return dynamicComponent.MoveInterval2;
                case 3: return dynamicComponent.MoveInterval3;
                case 4: return dynamicComponent.MoveInterval4;
                case 5: return dynamicComponent.MoveInterval5;
                default: return null;
            }
        }

        node.AppendChild(objectElement); //Place it into the Object node
        xml.Save(Application.dataPath + "/XML/dzip/level_xml/" + mapToOverride + ".xml"); //Apply the modification to the build-map.xml file}
    }
    void ConvertToObject(XmlNode node, XmlDocument xml, GameObject objectInScene, out XmlElement objectNode, bool localPosition)
    {
        objectNode = null;
        //Debug in log every object it writes
        if (debugObjectWriting)
            Debug.Log("Writing object : " + Regex.Replace(objectInScene.name, @" \((.*?)\)", string.Empty));

        if (objectInScene.name != "Camera")
        {
            string name = objectInScene.name;
            XmlElement element = xml.CreateElement("Object"); //Create a new node from scratch
            if (name != string.Empty)
            {
                element.SetAttribute("Name", Regex.Replace(name, @" \((.*?)\)", string.Empty)); //Add an name
            }

            if (localPosition)
            {
                element.SetAttribute("X", Math.Round(objectInScene.transform.localPosition.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                element.SetAttribute("Y", Math.Round(-objectInScene.transform.localPosition.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)

            }
            else
            {
                element.SetAttribute("X", Math.Round(objectInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
                element.SetAttribute("Y", Math.Round(-objectInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            }
            XmlElement contentElement = xml.CreateElement("Content");

            List<GameObject> ImageObjects = new List<GameObject>();

            // add image to the list
            foreach (UnityEngine.Transform child in objectInScene.transform)
            {
                if (child.gameObject.CompareTag("Image"))
                {
                    ImageObjects.Add(child.gameObject);
                }
            }

            // sort the list based on order in layer
            ImageObjects.Sort((a, b) =>
            {
                SpriteRenderer rendererA = a.GetComponent<SpriteRenderer>();
                SpriteRenderer rendererB = b.GetComponent<SpriteRenderer>();

                // Handle cases where SpriteRenderer might be null
                int orderA = rendererA != null ? rendererA.sortingOrder : 0;
                int orderB = rendererB != null ? rendererB.sortingOrder : 0;

                return orderA.CompareTo(orderB);
            });

            foreach (GameObject imageObject in ImageObjects)
            {
                XmlElement imageNode;
                ConvertToImage(node, xml, imageObject.gameObject, out imageNode, true);
                contentElement.AppendChild(imageNode);
            }
            foreach (UnityEngine.Transform childObject in objectInScene.transform)
            {
                if (childObject.gameObject.CompareTag("Untagged"))
                {

                    continue;

                }
                if (childObject.gameObject.CompareTag("Unused"))
                {

                    continue;

                }
                //check if the gameobject has specific tag

                if (childObject.gameObject.CompareTag("Out"))
                {

                    XmlElement objectNode1;
                    ConvertToOut(node, xml, childObject.gameObject, out objectNode1, true);
                    contentElement.AppendChild(objectNode1);

                }


                if (childObject.gameObject.CompareTag("In"))
                {
                    XmlElement objectNode1;
                    ConvertToIn(node, xml, childObject.gameObject, out objectNode1, true);
                    contentElement.AppendChild(objectNode1);

                }

                if (childObject.gameObject.CompareTag("Object"))
                {

                    XmlElement objectNode1;
                    ConvertToObject(node, xml, childObject.gameObject, out objectNode1, true);
                    contentElement.AppendChild(objectNode1);

                }

                else if (childObject.gameObject.CompareTag("Spawn"))
                {
                    //Platform

                    XmlElement spawnNode;
                    ConvertToSpawn(node, xml, childObject.gameObject, out spawnNode, true);
                    contentElement.AppendChild(spawnNode);

                }

                else if (childObject.gameObject.CompareTag("Platform"))
                {
                    //Platform

                    XmlElement platformNode;
                    ConvertToPlatform(node, xml, childObject.gameObject, out platformNode, true);
                    contentElement.AppendChild(platformNode);

                }

                else if (childObject.gameObject.CompareTag("Trapezoid"))
                {
                    // Trapezoid
                    XmlElement trapezoidNode;
                    ConvertToTrapezoid(node, xml, childObject.gameObject, out trapezoidNode, true);
                    contentElement.AppendChild(trapezoidNode);

                }
                else if (childObject.gameObject.CompareTag("Area"))
                {
                    XmlElement areaNode;
                    ConvertToArea(node, xml, childObject.gameObject, out areaNode, true);
                    contentElement.AppendChild(areaNode);
                }

                else if (childObject.gameObject.CompareTag("Trigger"))
                {
                    XmlElement triggerNode;
                    ConvertToTrigger(node, xml, childObject.gameObject, out triggerNode, true);
                    contentElement.AppendChild(triggerNode);

                }

                else if (childObject.gameObject.CompareTag("Dynamic Trigger"))
                {
                    XmlElement triggerNode;
                    ConvertToDynamicTrigger(node, xml, childObject.gameObject, out triggerNode, false);
                    contentElement.AppendChild(triggerNode);

                }

                else if (childObject.gameObject.CompareTag("Object Reference"))
                {

                    XmlElement objrefNode;
                    ConvertToObjectReference(node, xml, childObject.gameObject, out objrefNode, true);
                    contentElement.AppendChild(objrefNode);
                }

                else if (childObject.gameObject.CompareTag("Animation"))
                {
                    XmlElement animNode;
                    ConvertToAnimation(node, xml, childObject.gameObject, out animNode, true);
                    contentElement.AppendChild(animNode);
                }
                else if (childObject.gameObject.CompareTag("Camera"))
                {
                    XmlElement camNode;
                    ConvertToCamera(node, xml, childObject.gameObject, out camNode, true);
                    contentElement.AppendChild(camNode);
                }

                // Add content to the object
                element.AppendChild(contentElement);
            }
            objectNode = element;
        }


        else if (objectInScene.name == "Camera")
        {
            XmlElement element = xml.CreateElement("Camera"); //Create a new node from scratch
            element.SetAttribute("X", (objectInScene.transform.position.x * 100).ToString().Replace(',', '.')); //Add X position (Refit into the Vector units)
            element.SetAttribute("Y", (-objectInScene.transform.position.y * 100).ToString().Replace(',', '.')); // Add Y position (Negative because Vector see the world upside down)
            objectNode = element;
        }

    }

}