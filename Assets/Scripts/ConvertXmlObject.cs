using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEditor;

public class ConvertXmlObject : MonoBehaviour
{
    public string objectToConvert;
    public bool debugObjectFound;
    GameObject lastContent;
    GameObject actualObject;
    GameObject dummyObject;

    [MenuItem("Vectorier/Convert from objects.xml")]
    public static void ConvertXmlToObject()
    {
        Debug.Log("Converting...");

        string path = EditorUtility.OpenFilePanel("Select XML File", Application.dataPath, "xml");
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("No XML file selected!");
            return;
        }

        // Load the selected XML file
        XmlDocument obj = new XmlDocument();
        obj.Load(path);

        bool objectFound = false;
        int doc_num = 0;

        // Search for the selected object in the object XMLs
        while (!objectFound && doc_num < 3)
        {
            foreach (XmlNode node in obj.DocumentElement.SelectSingleNode("/Root/Objects"))
            {
                // Check if the object has the correct name
                if (node.Name == "Object" && node.Attributes.GetNamedItem("Name").Value == GameObject.FindObjectOfType<ConvertXmlObject>().objectToConvert)
                {
                    objectFound = true;

                    // Process each node in the object
                    foreach (XmlNode content in node.FirstChild)
                    {
                        GameObject.FindObjectOfType<ConvertXmlObject>().InstantiateObject(content);
                    }
                }
            }
            doc_num += 1;
        }

        if (!objectFound)
        {
            Debug.LogError("Object not found in the XML files.");
        }
        else
        {
            GameObject.FindObjectOfType<ConvertXmlObject>().actualObject = null;
            Debug.Log("Convert done!");
        }
    }

    void InstantiateObject(XmlNode content)
    {
        if (debugObjectFound)
        {
            Debug.Log($"Found {content.Name}: {content.Attributes.GetNamedItem("ClassName").Value}");
        }

        if (actualObject == null)
        {
            actualObject = Instantiate(new GameObject(objectToConvert), Vector3.zero, Quaternion.identity);
            DestroyImmediate(GameObject.Find(objectToConvert));
            actualObject.name = objectToConvert;
        }

        if (content.Name == "Image")
        {
            // Calculate the position
            Vector3 position = new Vector3(
                float.Parse(content.Attributes.GetNamedItem("X").Value) / 100,
                -float.Parse(content.Attributes.GetNamedItem("Y").Value) / 100,
                0
            );

            lastContent = Instantiate(
                dummyObject = new GameObject(content.Attributes.GetNamedItem("ClassName").Value),
                position,
                Quaternion.identity

            );

            // Load the sprite and apply
            SpriteRenderer spriteRenderer = lastContent.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Textures/" + content.Attributes.GetNamedItem("ClassName").Value);


            // Calculate scale based on Width and Height
            float width = float.Parse(content.Attributes.GetNamedItem("Width").Value);
            float height = float.Parse(content.Attributes.GetNamedItem("Height").Value);

            float originalWidth = spriteRenderer.sprite.texture.width;
            float originalHeight = spriteRenderer.sprite.texture.height;

            Vector3 scale = new Vector3(width / originalWidth, height / originalHeight, 1);
            lastContent.transform.localScale = scale;

            // Check if there is Matrix transformations
            if (content.HasChildNodes)
            {
                foreach (XmlNode matrixNode in content.SelectNodes("Properties/Static/Matrix"))
                {

                    ConvertFromMarmaladeMatrix(matrixNode, lastContent.transform, spriteRenderer, width, height);

                }
            }
        }

        lastContent.transform.parent = actualObject.transform;
        DestroyImmediate(dummyObject);

    }

    void ConvertFromMarmaladeMatrix(XmlNode matrixNode, UnityEngine.Transform transform, SpriteRenderer spriteRenderer, float xmlWidth, float xmlHeight)
    {
        float A = float.Parse(matrixNode.Attributes.GetNamedItem("A").Value);
        float D = float.Parse(matrixNode.Attributes.GetNamedItem("D").Value);

        // image dimensions
        float originalWidth = spriteRenderer.sprite.bounds.size.x * 100f;  // adjust width based on the sprite bounds
        float originalHeight = spriteRenderer.sprite.bounds.size.y * 100f; // adjust height based on the sprite bounds


        // Calculate scale
        float scaleX = A / originalWidth;
        float scaleY = D / originalHeight;


        // Adjust for potential double-scaling due to XML width/height attributes
        float expectedWidth = Mathf.Abs(scaleX * originalWidth);
        float expectedHeight = Mathf.Abs(scaleY * originalHeight);


        if (Mathf.Abs(expectedWidth - xmlWidth) > Mathf.Epsilon)
        {
            scaleX *= xmlWidth / expectedWidth;
        }

        if (Mathf.Abs(expectedHeight - xmlHeight) > Mathf.Epsilon)
        {
            scaleY *= xmlHeight / expectedHeight;
        }

        // flipping based on the matrix values
        bool flipX = scaleX < 0;
        bool flipY = scaleY < 0;


        // correct the scale to positive values (since flip is handled separately)
        scaleX = Mathf.Abs(scaleX);
        scaleY = Mathf.Abs(scaleY);


        transform.localScale = new Vector3(scaleX, scaleY, 1);
        spriteRenderer.flipX = flipX;
        spriteRenderer.flipY = flipY;


        // apply translation
        float Tx = float.Parse(matrixNode.Attributes.GetNamedItem("Tx").Value) / 100;
        float Ty = -float.Parse(matrixNode.Attributes.GetNamedItem("Ty").Value) / 100;
        transform.position += new Vector3(Tx, Ty, 0);
    }
}

