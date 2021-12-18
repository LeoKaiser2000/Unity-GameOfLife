using System.Collections;
using System.Collections.Generic;
using B83.Image.BMP;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class BitmapLoader : MonoBehaviour
{
    private readonly  BMPLoader bmpLoader = new BMPLoader();
    [SerializeField] private UnityEvent<BMPImage> OnBMPLoadedEvent;

    private string OpenFileExplorer()
    {
        return EditorUtility.OpenFilePanel("Open bitmap file", "", "bmp");
    }
    
    private BMPImage ReadMPPFile(string path)
    {
        return bmpLoader.LoadBMP(path);
    }

    public void LoadUserBMPFile()
    {
        var path = OpenFileExplorer();
        if (string.IsNullOrEmpty(path))
            return;
        var image = ReadMPPFile(path);
        if (image == null)
            return;
        OnBMPLoadedEvent.Invoke(image);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
