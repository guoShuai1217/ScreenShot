/*
 *		Description : 
 *
 *		CreatedBy : guoShuai
 *
 *		DataTime : 2018.08
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LCPrinter;

public class PrintPhoto : MonoBehaviour
{

    public Button printBtn;
    public Button PDFBtn;
    public Button returnBtn;
    private PrintOrPDF _printOrPDF;
    public Transform BtnPanel;
    public Transform ImgPanel;
    public RawImage[] ImagePanel;
   
    public Button sureBtn;

    string outPath;
    private void Start()
    {
        outPath =  Application.streamingAssetsPath + "/ScreenImage";
        printBtn.onClick.AddListener(()=> { OnClickPrint(PrintOrPDF.Print); });
        PDFBtn.onClick.AddListener(() => { OnClickPrint(PrintOrPDF.PDF); });
        sureBtn.onClick.AddListener(ScreenView);
        returnBtn.onClick.AddListener(() =>
        {
            BtnPanel.gameObject.SetActive(true);
            ImgPanel.gameObject.SetActive(false);
        });
    }

    private void OnClickPrint(PrintOrPDF prit)
    {
        BtnPanel.gameObject.SetActive(false);
        ImgPanel.gameObject.SetActive(true);

        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);

        string _path = outPath + "/ScreenShot.png";//生成临时图片的地址
        if (File.Exists(_path))
        {
            File.Delete(_path);
            Debug.Log("删除旧图片成功");
        }
        ScreenCapture.CaptureScreenshot(_path, 0);
        StartCoroutine(WaitFew());

        _printOrPDF = prit;

    }

    private Camera ca;
    /// <summary>
    /// 这个协程是为了防止临时截图不成功
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitFew()
    {
        GameObject cam = GameObject.Find("thumbnailCamera");
        if (cam == null)
        {
            cam = new GameObject();
            cam.name = "thumbnailCamera";
            ca = cam.AddComponent<Camera>();
        }
        else
        {
            ca = cam.GetComponent<Camera>();
        }
        GameObject house2d = GameObject.FindGameObjectWithTag("Exterior");
        MeshFilter m = house2d.GetComponent<MeshFilter>(); // 没有这个组件可以不加 , 直接找到中心点的物体即可

        yield return new WaitForFixedUpdate();
        ca.transform.position = m.mesh.bounds.center;
        ca.transform.position = new Vector3(0, 7, 0);
        ca.transform.eulerAngles = new Vector3(90, 0, 0);
        // Destroy(ca[0].GetComponent<AudioListener>());
        //thumbnailCamera.orthographic = true;
        ca.orthographicSize = Vector3.Distance(m.mesh.bounds.center + m.mesh.bounds.extents, m.mesh.bounds.center) * 0.85f;



        ImagePanel[0].texture = CaptureCamera(ca, new Rect(0, 0, Screen.width, Screen.height));
        ImagePanel[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        ca.transform.position = m.mesh.bounds.center;
        ca.transform.position = new Vector3(7, 7, -7);
        ca.transform.LookAt(m.mesh.bounds.center);
        // Destroy(ca[1].GetComponent<AudioListener>());
        //thumbnailCamera.orthographic = true;
        ca.orthographicSize = Vector3.Distance(m.mesh.bounds.center + m.mesh.bounds.extents, m.mesh.bounds.center) * 0.85f;
        // print(ca[1].transform.position + "***" + ca[1].transform.eulerAngles);

        ImagePanel[1].texture = CaptureCamera(ca, new Rect(0, 0, Screen.width, Screen.height));
        ImagePanel[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);

        ca.transform.position = m.mesh.bounds.center;
        ca.transform.position = new Vector3(0, 7, -7);
        ca.transform.LookAt(m.mesh.bounds.center);
        //Destroy(ca[2].GetComponent<AudioListener>());
        //thumbnailCamera.orthographic = true;
        ca.orthographicSize = Vector3.Distance(m.mesh.bounds.center + m.mesh.bounds.extents, m.mesh.bounds.center) * 0.85f;
        //  print(ca[2].transform.position + "***" + ca[0].transform.eulerAngles);


        ImagePanel[2].texture = CaptureCamera(ca, new Rect(0, 0, Screen.width, Screen.height));
        ImagePanel[2].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);

        ca.transform.position = m.mesh.bounds.center;
        ca.transform.position = new Vector3(-7, 7, 7);
        ca.transform.LookAt(m.mesh.bounds.center);
        //Destroy(ca[3].GetComponent<AudioListener>());
        //thumbnailCamera.orthographic = true;
        ca.orthographicSize = Vector3.Distance(m.mesh.bounds.center + m.mesh.bounds.extents, m.mesh.bounds.center) * 0.85f;
        //print(ca[3].transform.position + "***" + ca[0].transform.eulerAngles);


        ImagePanel[3].texture = CaptureCamera(ca, new Rect(0, 0, Screen.width, Screen.height));
        ImagePanel[3].gameObject.SetActive(true);
    }

    //得到临时图片
    Sprite GetPicture()
    {
        string _path = UnityEngine.Application.streamingAssetsPath + "/ScreenImage/";//生成临时图片的地址
        DirectoryInfo dir = new DirectoryInfo(_path);
        FileInfo[] files = dir.GetFiles("*.png"); //获取所有文件信息
        Sprite _sprite = new Sprite();
        foreach (FileInfo file in files)
        {
            FileStream fs = new FileStream(_path + file.Name, FileMode.Open);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(buffer);
            tex.Apply();

            Sprite s = ChangeToSprite(tex);
            s.name = file.Name;
            _sprite = s;
        }
        return _sprite;
    }

    Sprite ChangeToSprite(Texture2D tex)
    {
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    /// <summary>
    /// 获取当前屏幕图片,并开启等待开启PDF
    /// </summary>
    public void ScreenView()
    {
        string _path = UnityEngine.Application.streamingAssetsPath + "/Screen/ScreenShot.png";//生成图片的地址
        if (File.Exists(_path))
        {
            File.Delete(_path);
            Debug.Log("删除旧图片成功");
        }
        ScreenCapture.CaptureScreenshot(_path, 0);
        Debug.Log("生成图片路径为：" + _path);
        StartCoroutine(WaitCoroutine());
    }
    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (_printOrPDF == PrintOrPDF.PDF)
        {
            StartPDF();
        }
        if (_printOrPDF == PrintOrPDF.Print)
        {
            PrintFile();
        }
    }

    /// <summary>
    /// 打印功能
    /// </summary>
    public void PrintFile()
    {

        string url = Application.streamingAssetsPath + "/Screen/ScreenShot.png";//打印生成的地址
        System.Diagnostics.Process process = new System.Diagnostics.Process(); //系统进程
        process.StartInfo.CreateNoWindow = false; //不显示调用程序窗口
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;//
        process.StartInfo.UseShellExecute = true; //采用操作系统自动识别模式
        process.StartInfo.FileName = url; //要打印的文件路径
        process.StartInfo.Verb = "print"; //指定执行的动作，打印：print 打开：open …………
        process.Start(); //开始打印
    }

    //把本地图片转换为Texture2D,并开启保存PDF功能
    void StartPDF()
    {

        string path = Application.streamingAssetsPath + "/Screen/ScreenShot.png";
        FileStream fs = new FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        byte[] thebytes = new byte[fs.Length];
        fs.Read(thebytes, 0, (int)fs.Length);
        Texture2D _texture2D = new Texture2D(1, 1);
        _texture2D.LoadImage(thebytes);
        LCPrinter.Print.PrintTexture(_texture2D.EncodeToPNG(), 1, "");//保存PDF功能
        Debug.Log("开启保存PDF");
    }

    #region 相机截图
    /// <summary>  
    /// 对相机截图。   
    /// </summary>  
    /// <returns>The screenshot2.</returns>  
    /// <param name="camera">Camera.要被截屏的相机</param>  
    /// <param name="rect">Rect.截屏的区域</param>  
    Texture2D CaptureCamera(Camera camera, Rect rect)
    {
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera.targetTexture = rt;
        camera.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
        //ps: camera2.targetTexture = rt;  
        //ps: camera2.Render();  
        //ps: -------------------------------------------------------------------  

        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();

        // 重置相关参数，以使用camera继续在屏幕上显示  
        camera.targetTexture = null;
        //ps: camera2.targetTexture = null;  
        RenderTexture.active = null; // JC: added to avoid errors  
        GameObject.Destroy(rt);
        // 最后将这些纹理数据，成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + "/Screenshot.png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));

        return screenShot;
    }
    #endregion

}
public enum PrintOrPDF
{
    Print,//打印
    PDF   //生成PDF
}

