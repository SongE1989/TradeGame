using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using NewEditorNS;
using System.Drawing;
using LitJson;
using System.Reflection;
using System.Drawing.Drawing2D;

public static class SongeUtil
{

    public static bool IsPic(string fileName)
    {
        string postFix = SongeUtil.GetFilePostfix(fileName);
        return postFix == "png"
            || postFix == "PNG"
            || postFix == "jpg"
            || postFix == "JPG"
            || postFix == "jpeg"
            || postFix == "JPEG";
    }

    /// <summary>添加进key-value(list)型字典, 并确保列表非空与不重复添加</summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="dic"></param>
    /// <param name="key"></param>
    /// <param name="tar"></param>
    /// <returns></returns>
    public static Dictionary<T1, List<T2>> AddToList<T1, T2>(this Dictionary<T1, List<T2>> dic, T1 key, T2 tar)
    {
        if (!dic.ContainsKey(key))
            dic.Add(key, new List<T2>());
        List<T2> list = dic[key];
        if (!list.Contains(tar))
            list.Add(tar);
        return dic;
    }

    public static List<T> RemoveFromList<T>(this List<T> list, Predicate<T> pred)
    {
        List<T> deleteList = new List<T>();
        for (int i = 0, length = list.Count; i < length; i++)
        {
            if (pred(list[i]))
                deleteList.Add(list[i]);
        }
        for (int i = 0, length = deleteList.Count; i < length; i++)
        {
            list.Remove(deleteList[i]);
        }
        return list;
    }

    /// <summary>是否在NGUI的输入状态(判断是否有选中物)</summary>
    public static bool IsInputting()
    {
        return UICamera.selectedObject == null ?
            true : UICamera.selectedObject.GetComponent<UIInput>() != null;
    }

    /// <summary>替换/添加, 如果字典中已有则替换值</summary>
    public static Dictionary<T1, T2> AddRep<T1, T2>(this Dictionary<T1, T2> dic, T1 key, T2 value)
    {
        if (dic.ContainsKey(key))
            dic[key] = value;
        else
            dic.Add(key, value);
        return dic;
    }

    /// <summary>获取法向量</summary>
    public static Vector3 GetNormalVector(Vector3 va, Vector3 vb, Vector3 vc)
    {
        //平面方程Ax+BY+CZ+d=0 行列式计算
        float A = va.y * vb.z + vb.y * vc.z + vc.y * va.z - va.y * vc.z - vb.y * va.z - vc.y * vb.z;
        float B = -(va.x * vb.z + vb.x * vc.z + vc.x * va.z - vc.x * vb.z - vb.x * va.z - va.x * vc.z);
        float C = va.x * vb.y + vb.x * vc.y + vc.x * va.y - va.x * vc.y - vb.x * va.y - vc.x * vb.y;
        float D = -(va.x * vb.y * vc.z + vb.x * vc.y * va.z + vc.x * va.y * vb.z - va.x * vc.y * vb.z - vb.x * va.y * vc.z - vc.x * vb.y * va.z);
        float E = Mathf.Sqrt(A * A + B * B + C * C);
        Vector3 res = new Vector3(A / E, B / E, C / E);
        return (res);
    }

    public static void addListener(this MonoBehaviour mono, List<EventDelegate> delegateList, string callbackFuncName, params UnityEngine.Object[] paramArr)
    {
        EventDelegate ed = new EventDelegate(mono, callbackFuncName);
        for (int i = 0, length = paramArr.Length; i < length; i++)
        {
            ed.parameters[i] = new EventDelegate.Parameter();
            ed.parameters[i].obj = paramArr[i];
        }
        EventDelegate.Add(delegateList, ed, false);
    }

    public static T FindItem<T>(this IEnumerable<T> enu, Predicate<T> judgeFunc)
    {
        foreach (var item in enu)
        {
            if (judgeFunc(item))
                return item;
        }
        return default(T);
    }
    public static List<T> FindAllItem<T>(this IEnumerable<T> enu, Predicate<T> judgeFunc)
    {
        List<T> list = new List<T>();
        foreach (var item in enu)
        {
            if (judgeFunc(item))
                list.Add(item);
        }
        return list;
    }

    /// <summary>不需要file前缀, No "file:///"!!! </summary>
    public static Texture2D GetTexture2DFromLoacl(string filePath)
    {
        //filePath = filePath.Replace(@"\", @"/");
        if (!File.Exists(filePath))
        {
            Debug.Log("找不到文件!\r\n" + filePath);
            NGUIDebug.Log("找不到文件!\r\n" + filePath);
            return null;
        }
        FileStream fs = null;
        try
        {
            fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
        catch (Exception exc)
        {
            NGUIDebug.Log("exc1: " + exc.Message);
        }
        NGUIDebug.Log("GT1" + (fs == null));
        Image img = null;
        try
        {
            img = Image.FromStream(fs);
        }
        catch (Exception exc)
        {
            NGUIDebug.Log("exc2: " + exc.Message);
        }
        NGUIDebug.Log("GT2" + (img == null));
        //Image img = Image.FromFile(_path); //方法二加载图片方式。

        MemoryStream ms = new MemoryStream();
        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        Texture2D _tex2 = new Texture2D(img.Width, img.Height);
        NGUIDebug.Log("GT3" + (_tex2 == null));
        _tex2.LoadImage(ms.ToArray());
        NGUIDebug.Log("GT4" + (_tex2 == null));

        fs.Close();
        fs.Dispose();
        ms.Close();
        ms.Dispose();
        return _tex2;
    }

    /// <summary>过滤并返回一个新数组</summary>
    public static T[] filter<T>(this T[] arr, Func<T, bool> filterFunc)
    {
        List<T> list = new List<T>();
        for (int i = 0, length = arr.Length; i < length; i++)
        {
            if (filterFunc(arr[i]))
                list.Add(arr[i]);
        }
        return list.ToArray();
    }
    /// <summary>过滤并返回一个新数组</summary>
    public static List<T> filter<T>(this List<T> arr, Func<T, bool> filterFunc)
    {
        List<T> list = new List<T>();
        for (int i = 0, length = arr.Count; i < length; i++)
        {
            if (filterFunc(arr[i]))
                list.Add(arr[i]);
        }
        return list;
    }

    public static void openFileDialog(Action<string> onFileOpen)
    {
        //Debug.Log("openDialog");
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = System.Runtime.InteropServices.Marshal.SizeOf(ofn);

        ofn.filter = "All Files\0*.*\0\0";

        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;

        ofn.initialDir = UnityEngine.Application.dataPath;//默认路径  

        ofn.title = "Open Project";

        ofn.defExt = "JPG";//显示文件的类型  
        //注意 一下项目不一定要全选 但是0x00000008项不要缺少  
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

        if (WindowDll.GetOpenFileName(ofn))
        {
            //Debug.Log("Selected file with full path: {0}" + ofn.file);

            //this.pathOver(ofn.file);
            if (onFileOpen != null)
                onFileOpen(ofn.file);
        }
    }

    public static bool IsCombineComp(string prefabType)
    {
        if (prefabType == null)
            return false;
        string prefix = "combineComp_";
        return prefabType.Length >= prefix.Length && prefabType.Substring(0, prefix.Length) == prefix;
    }
    public static bool IsBuildClone(string prefabType)
    {
        string prefix = "buildingClone_";
        return prefabType.Length >= prefix.Length && prefabType.Substring(0, prefix.Length) == prefix;
    }

    public static Vector3 GetScaledVector(this Vector3 tar, float multi)
    {
        return new Vector3(tar.x * multi, tar.y * multi, tar.z * multi);
    }
    public static string ForeachToString<T>(this ICollection<T> list, string sep = ", ")
    {
        string res = "";
        if (list is List<T>)
        {
            for (int i = 0, length = list.Count; i < length; i++)
            {
                List<T> _list = list as List<T>;
                res += _list[i] == null ? null : _list[i].ToString();
                if (i != length - 1)
                    res += sep;
            }
        }
        else
        {
            foreach (var item in list)
            {
                if (item != null)
                    res += item.ToString();
                else
                    res += null;
                res += sep;
            }
        }
        return res;
    }

    public static Bounds? GetBounds(GameObject go)
    {
        Bounds? bounds = null;
        //bounds.
        Bounds ab;

        SongeUtil.forAllChildren(go, tar =>
        {
            if (tar.GetComponent<Renderer>() != null)
            {
                Bounds b = tar.GetComponent<Renderer>().bounds;
                if (bounds == null)
                    bounds = b;
                else
                {
                    ab = bounds.Value;//对bounds.Value的改变不能作用到bounds上 需要中间变量
                    ab.Encapsulate(b);
                    bounds = ab;
                }
            }
        });
        return bounds;
    }

    public static string GetUnderArea(Vector3 pos)
    {
        string areaName = null;
        RaycastHit rh = new RaycastHit();
        bool isHit = SongeUtil.GetPointOnGround(pos, ref rh);
        if (isHit)
        {
            Transform tr = rh.transform;
            AreaScript areaSc = tr.GetComponentInParent<AreaScript>();
            if (areaSc != null)
                areaName = areaSc.AreaName;
        }
        return areaName;
    }

    public static string GetUnderAreaName(Vector3 pos)
    {
        string areaName = null;
        RaycastHit rh = new RaycastHit();
        bool isHit = SongeUtil.GetPointOnGround(pos, ref rh);
        if (isHit)
        {
            Transform tr = rh.transform;
            AreaVO areaSc = tr.GetComponentInParent<AreaVO>();
            if (areaSc != null)
                areaName = areaSc.AreaName;
        }
        return areaName;
    }

    public static Vector3 GetMoveInput(bool allowArrow = true)
    {
        if(UICamera.inputHasFocus)
        {
            return Vector3.zero;
        }
        else
        {
            float kz = 0;
            if (allowArrow)
            {
                kz += Input.GetKey(KeyCode.UpArrow) ? 1f : 0f * 1f;
                kz += Input.GetKey(KeyCode.DownArrow) ? -1f : 0f * 1f;
            }
            kz += Input.GetKey(KeyCode.W) ? 1f : 0f * 1f;
            kz += Input.GetKey(KeyCode.S) ? -1f : 0f * 1f;
            float kx = 0;
            if (allowArrow)
            {
                kx -= Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f * 1f;
                kx -= Input.GetKey(KeyCode.RightArrow) ? 1f : 0f * 1f;
            }
            kx -= Input.GetKey(KeyCode.A) ? -1f : 0f * 1f;
            kx -= Input.GetKey(KeyCode.D) ? 1f : 0f * 1f;
            return new Vector3(kx, 0, kz);
        }
    }

    /// <summary>获取朝目标方向前进后退左右平移后, 坐标的改变</summary>
    /// <param name="rotH">水平面旋转方向</param>
    /// <param name="dirX">左右移动距离</param>
    public static Vector3 MoveTowards(float rotH, float dirX, float dirZ)
    {
        float dx = dirX * Mathf.Sin(rotH) + dirZ * Mathf.Cos(rotH);
        float dz = dirX * Mathf.Cos(rotH) - dirZ * Mathf.Sin(rotH);
        return new Vector3(dx, 0, dz);
    }
    public static Vector3 MoveTowards(float rotH, Vector3 dir)
    {
        float dx = dir.x * Mathf.Sin(rotH) + dir.z * Mathf.Cos(rotH);
        float dz = dir.x * Mathf.Cos(rotH) - dir.z * Mathf.Sin(rotH);
        return new Vector3(dx, 0, dz);
    }

    /// <summary>获取文件名后缀</summary>
    public static string GetFilePostfix(string fileName)
    {
        string res;
        if (fileName.IndexOf(".") == -1)
            res = "";
        else
        {
            string[] ss = fileName.Split(new char[1] { '.' });
            res = ss[ss.Length - 1];
        }
        return res;
    }

    public static string GetFolderPath(string path, bool fullPath = true)
    {
        path = path.Replace(@"\", @"/");
        if(fullPath)//获取全路径
        {
            if (path.LastIndexOf(@"/") == path.Length - 1)
                return GetFolderPath(path.Substring(0, path.Length - 1));
            else
                return path.Substring(0, path.LastIndexOf(@"/") + 1);
        }
        else//获取父级文件夹名
        {
            string[] strArr = path.Split('/');
            if (path.LastIndexOf(@"/") == path.Length - 1)
                return strArr[strArr.Length - 2];
            else
                return strArr[strArr.Length - 1];
        }
    }

    public static string GetFileName(string path, bool needPostfix = false)
    {
        path = path.Replace(@"\", @"/");
        string fileFolderPath = path.Substring(0, path.LastIndexOf(@"/") + 1);

        string fileName = path.Substring(path.LastIndexOf("/") + 1, path.Length - fileFolderPath.Length);
        if (needPostfix)
            return fileName;
        else
            return fileName.Substring(0, fileName.LastIndexOf("."));
    }

    public static void ChangeShader(GameObject tar)
    {
        forAllChildren(tar, go =>
        {
            Renderer r = go.GetComponent<Renderer>();
            if (r != null)
            {
                for (int i = 0, len = r.sharedMaterials.Length; i < len; i++)
                {
                    Material mat = r.sharedMaterials[i];
                    if (mat != null && mat.shader != null)
                    {
                        if (mat.shader.name != "Nature/SpeedTree" && mat.shader.name != "Nature/SpeedTree Billboard")
                        {
                            if (mat.shader.name.IndexOf("Transparent") != -1)
                                mat.shader = Shader.Find("MyShader/Transparent/DoubleSided_SelfIllum");
                            else
                                mat.shader = Shader.Find("MyShader/Opaque/SelfIllum");
                        }
                    }
                }
            }
        }, false);
    }

    /// <summary>返回目标向量在X-Z平面旋转(增加)angle(弧度)后的单位向量</summary>
    public static Vector3 RotateVecotrAtXZ(Vector3 tar, float angle)
    {
        float ang = Mathf.Atan2(tar.z, tar.x);
        ang = ang + angle;
        float newX = 1f * Mathf.Cos(ang);
        float newZ = 1f * Mathf.Sin(ang);
        return new Vector3(newX, tar.y, newZ); ;
    }

    #region 扩展方法
    public enum EnumVector { x, y, z }
    public static void SetLocalPos(this GameObject tar, float value, EnumVector dir)
    {
        SetLocalPos(tar.transform, value, dir);
    }
    public static void SetLocalPos(this Transform tar, float value, EnumVector dir)
    {
        float newValueX = dir == EnumVector.x ? value : tar.localPosition.x;
        float newValueY = dir == EnumVector.y ? value : tar.localPosition.y;
        float newValueZ = dir == EnumVector.z ? value : tar.localPosition.z;
        tar.localPosition = new Vector3(newValueX, newValueY, newValueZ);
    }

    public static void SetVec(ref Vector3 tar, float value, EnumVector dir)
    {
        float newValueX = dir == EnumVector.x ? value : tar.x;
        float newValueY = dir == EnumVector.y ? value : tar.y;
        float newValueZ = dir == EnumVector.z ? value : tar.z;
        tar.Set(newValueX, newValueY, newValueZ);
    }

    #endregion

    public static Vector3 Approach(Vector3 fromPos, Vector3 toPos, float delta, out bool isArrive)
    {
        Vector3 dir = toPos - fromPos;
        isArrive = dir.magnitude <= delta;
        Vector3 deltaPos = Vector3.ClampMagnitude(dir, delta);
        return isArrive ? toPos : fromPos + deltaPos;
    }

    public static void addClickEventListener(MonoBehaviour listener, string methodName, GameObject tarObj)
    {
        if (tarObj.GetComponent<UIButton>() == null)
            return;
        EventDelegate ev = new EventDelegate(listener, methodName);
        ev.parameters[0] = new EventDelegate.Parameter();
        ev.parameters[0].obj = tarObj;
        EventDelegate.Add(tarObj.GetComponent<UIButton>().onClick, ev);
    }

    /// <summary>
    /// 侦听子集的UIButton.onClick事件
    /// </summary>
    /// <param name="listener">事件接受者</param>
    /// <param name="target">事件发送者</param>
    /// <param name="callback">回调函数</param>
    public static void addChildClickEventListener(MonoBehaviour listener, Transform target, string callback)
    {
        foreach (Transform childTran in target)
        {
            addClickEventListener(listener, callback, childTran.gameObject);

            addChildClickEventListener(listener, childTran, callback);
        }
    }

    //示例 eularAngle = new Vector3( -GetRotatV, GetRotateY, 0)
    /// <summary>返回Vector3(0, 0, 1)指向目标向量, Y轴需要旋转角度(360)</summary>
    public static float GetRotateY(Vector3 dir)
    {
        return Mathf.Atan2(dir.x, dir.z) / Mathf.PI * 180;
    }
    //示例 eularAngle = new Vector3( -GetRotatV, GetRotateY, 0)
    /// <summary>获取仰角</summary>
    public static float GetRotatV(Vector3 dir)
    {
        float len2 = dir.x * dir.x + dir.z * dir.z;
        float len = Mathf.Sqrt(len2);
        float atan = Mathf.Atan2(dir.y, len);
        return atan / Mathf.PI * 180;
    }

    /// <summary>获取鼠标在水平面的投影点</summary>
    public static Vector3 GetMousePointOnHorizontal(Camera tarCamera = null)
    {
        if (tarCamera == null)
            tarCamera = Camera.main;
        Ray ray = tarCamera.ScreenPointToRay(Input.mousePosition);
        float ratio = -ray.origin.y / ray.direction.y;
        return ray.origin + ray.direction * ratio;
    }

    /// <summary>获取鼠标在ground_layer层的投影点</summary>
    public static Vector3 GetMousePointOnGroundLayer(Camera tarCamera = null)
    {
        if (tarCamera == null)
            tarCamera = Camera.main;
        Ray ray = tarCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rh;
        if (Physics.Raycast(ray, out rh, 9999f, LayerMask.GetMask(new string[] { LayerTagManager.ground_layer })))
            return rh.point;
        else
            return GetMousePointOnHorizontal(tarCamera);
    }

    public static bool GetPointOnGround(Vector3 vec, ref RaycastHit rh)
    {
        Ray ray = new Ray(new Vector3(vec.x, 999f, vec.z), Vector3.down);
        return Physics.Raycast(ray, out rh, 9999f, LayerMask.GetMask(new string[] { LayerTagManager.ground_layer }));
    }

    /// <summary>
    /// 按层进行射线检测
    /// </summary>
    /// <param name="hit">导出碰撞信息</param>
    /// <param name="LayerNameList">层名称列表</param>
    /// <param name="targetCamera">目标相机</param>
    /// <returns>是否碰撞</returns>
    public static bool RayCastByLayer(ref RaycastHit hit, string[] LayerNameList = null, Camera targetCamera = null)
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] raycastHitArr;
        if (LayerNameList == null)
            raycastHitArr = Physics.RaycastAll(ray);
        else
            raycastHitArr = Physics.RaycastAll(ray, 999999, LayerMask.GetMask(LayerNameList));
        foreach (RaycastHit rayCastHit in raycastHitArr)
        {
            hit = rayCastHit;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取海拔
    /// </summary>
    public static bool GetAltitude(Vector3 pos, ref float height)
    {
        Ray ray = new Ray(new Vector3(pos.x, 500, pos.z), Vector3.down);
        RaycastHit[] raycastHitArr = Physics.RaycastAll(ray, 999999f, LayerMask.GetMask(new string[] { LayerTagManager.ground_layer }));
        foreach (RaycastHit rayCastHit in raycastHitArr)
        {
            height = rayCastHit.point.y;
            return true;
        }
        return false;
    }


    /// <summary>
    /// 遍历Transform子集并执行operate
    /// 用例 1: SongeUtil.forAllChildren(gameObject,tar => {tar.transform.position = Vector3.zero;});
    /// </summary>
    public static void forAllChildren(GameObject target, Action<GameObject> operate, bool includeTarget = true)
    {
        //用例 2
        //System.Action<GameObject> setShow = null;
        //setShow += (GameObject tar) =>{};
        //SongeUtil.forAllChildren(wall, setShow);
        if (target == null)
            return;

        if (includeTarget)
            operate(target);
        for (int i = 0, length = target.transform.childCount; i < length; i++)
        {
            Transform childTran = target.transform.GetChild(i);
            operate(childTran.gameObject);
            forAllChildren(childTran.gameObject, operate, false);
        }
    }

    public static void followMouse(Transform targetTrans, Camera uiCamera = null)
    {
        Vector3 pos = Input.mousePosition;
        //if (uiCamera == null)
        //    uiCamera = UICamera.list[0].gameObject.GetComponent<Camera>();

        if (uiCamera != null)
        {
            pos.x = Mathf.Clamp01(pos.x / Screen.width);
            pos.y = Mathf.Clamp01(pos.y / Screen.height);
            targetTrans.position = uiCamera.ViewportToWorldPoint(pos);

            // For pixel-perfect results
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
            if (uiCamera.isOrthoGraphic)
#else
            if (uiCamera.orthographic)
#endif
            {
                Vector3 lp = targetTrans.localPosition;
                lp.x = Mathf.Round(lp.x);
                lp.y = Mathf.Round(lp.y);
                targetTrans.localPosition = lp;
            }
        }
        else
        {
            // Simple calculation that assumes that the camera is of fixed size
            pos.x -= Screen.width * 0.5f;
            pos.y -= Screen.height * 0.5f;
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            targetTrans.localPosition = pos;
        }
    }

    /// <summary>仅当该键按下时, 返回true</summary>
    public static bool IsMouseButtonOnly(int button)
    {
        Dictionary<int, bool> dic = new Dictionary<int, bool>();
        dic.Add(0, Input.GetMouseButton(0));
        dic.Add(1, Input.GetMouseButton(1));
        dic.Add(2, Input.GetMouseButton(2));
        return (dic[0] == (button == 0)) && (dic[1] == (button == 1)) && (dic[2] == (button == 2));
    }

}
namespace songeP
{
    public class FileUtil
    {
        /// <summary>打开文件选取对话框</summary>
        public static void OpenFileDialog(Action<string> onFileOpen)
        {
            //Debug.Log("openDialog");
            OpenFileName ofn = new OpenFileName();

            ofn.structSize = System.Runtime.InteropServices.Marshal.SizeOf(ofn);

            ofn.filter = "All Files\0*.*\0\0";

            ofn.file = new string(new char[256]);

            ofn.maxFile = ofn.file.Length;

            ofn.fileTitle = new string(new char[64]);

            ofn.maxFileTitle = ofn.fileTitle.Length;

            ofn.initialDir = UnityEngine.Application.dataPath;//默认路径  

            ofn.title = "Open Project";

            ofn.defExt = "JPG";//显示文件的类型  
            //注意 一下项目不一定要全选 但是0x00000008项不要缺少  
            ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

            if (WindowDll.GetOpenFileName(ofn))
            {
                //Debug.Log("Selected file with full path: {0}" + ofn.file);

                //this.pathOver(ofn.file);
                if (onFileOpen != null)
                    onFileOpen(ofn.file);
            }
        }

        /// <summary>获取文件名后缀</summary>
        public static string GetFilePostfix(string fileName)
        {
            string res;
            if (fileName.IndexOf(".") == -1)
                res = "";
            else
            {
                string[] ss = fileName.Split(new char[1] { '.' });
                res = ss[ss.Length - 1];
            }
            return res;
        }

        public static string GetFolderPath(string path)
        {
            path = path.Replace(@"\", @"/");
            if (path.LastIndexOf(@"/") == path.Length - 1)
                return GetFolderPath(path.Substring(0, path.Length - 1));
            else
                return path.Substring(0, path.LastIndexOf(@"/") + 1);
        }

        public static string GetFileName(string path, bool needPostfix = false)
        {
            path = path.Replace(@"\", @"/");
            string fileFolderPath = path.Substring(0, path.LastIndexOf(@"/") + 1);

            string fileName = path.Substring(path.LastIndexOf("/") + 1, path.Length - fileFolderPath.Length);
            if (needPostfix)
                return fileName;
            else
                return fileName.Substring(0, fileName.LastIndexOf("."));
        }
    }
}

/// <summary>物体变色工具 初次变色时 自动记录原来颜色 </summary>
public class ColorManager
{
    public static UnityEngine.Color GrayGreen = new UnityEngine.Color(60f / 255f, 68f / 255f, 24f / 255f);
    public static UnityEngine.Color Brown = new UnityEngine.Color(82f / 255f, 56f / 255f, 12f / 255f);
    public static UnityEngine.Color Orange = new UnityEngine.Color(255f / 255f, 128f / 255f, 71f / 255f);

    static ColorManager cm;
    public static ColorManager GetIns()
    {
        if (cm == null)
            cm = new ColorManager();
        return cm;
    }

    Dictionary<GameObject, Dictionary<Material, UnityEngine.Color>> _ColorDic = new Dictionary<GameObject, Dictionary<Material, UnityEngine.Color>>();
    Dictionary<GameObject, Dictionary<Material, UnityEngine.Color>> _EmissionDic = new Dictionary<GameObject, Dictionary<Material, UnityEngine.Color>>();

    /// <summary>还原原色</summary>
    public void RevertColor(MonoBehaviour tar)
    {
        if (tar != null)
            RevertColor(tar.gameObject);
    }
    /// <summary>还原原色</summary>
    public void RevertColor(GameObject tar)
    {
        if (tar == null)
            return;
        if (!_ColorDic.ContainsKey(tar))
        {
            //Debug.Log("在还原 [" + tar.name + "] 颜色之前未记录!");
            return;
        }

        SongeUtil.forAllChildren(tar, go =>
        {
            Renderer r = go.GetComponent<Renderer>();
            if (r != null)
            {
                foreach (var mat in r.materials)
                {
                    if (mat != null)
                    {
                        if (_ColorDic[tar].ContainsKey(mat))
                        {
                            mat.SetColor("_Color", _ColorDic[tar][mat]);
                            if (mat.HasProperty("_Emission"))
                                mat.SetColor("_Emission", _EmissionDic[tar][mat]);
                        }
                    }
                }
            }
        }, true);
    }

    /// <summary>改变颜色</summary>
    public void ChangeColor(MonoBehaviour tar, UnityEngine.Color color)
    {
        if (tar != null)
            ChangeColor(tar.gameObject, color);
    }
    /// <summary>改变颜色</summary>
    public void ChangeColor(GameObject tar, UnityEngine.Color color)
    {
        if (tar == null)
            return;
        bool hasRecord = _ColorDic.ContainsKey(tar);
        if (!hasRecord)
        {
            _ColorDic.Add(tar, new Dictionary<Material, UnityEngine.Color>());
            _EmissionDic.Add(tar, new Dictionary<Material, UnityEngine.Color>());
        }

        SongeUtil.forAllChildren(tar, go =>
        {
            Renderer r = go.GetComponent<Renderer>();
            if (r != null)
            {
                foreach (var mat in r.materials)
                {
                    if (mat != null)
                    {
                        if (!hasRecord)
                        {
                            _ColorDic[tar].Add(mat, mat.GetColor("_Color"));
                            if (mat.HasProperty("_Emission"))
                                _EmissionDic[tar].Add(mat, mat.GetColor("_Emission"));
                        }

                        mat.SetColor("_Color", color);
                        if (mat.HasProperty("_Emission"))
                            mat.SetColor("_Emission", color);
                    }
                }
            }
        }, true);
    }

    /// <summary>注销记录</summary>
    public void UnregeisterColor(GameObject tar)
    {
        if (_ColorDic.ContainsKey(tar))
        {
            List<Material> removeList = new List<Material>();
            if (_ColorDic[tar] != null)
            {
                foreach (var mat in _ColorDic[tar].Keys)
                    removeList.Add(mat);
                foreach (var mat in removeList)
                    _ColorDic[tar].Remove(mat);
            }
            removeList = new List<Material>();
            if (_EmissionDic[tar] != null)
            {
                foreach (var mat in _EmissionDic[tar].Keys)
                    removeList.Add(mat);
                foreach (var mat in removeList)
                    _EmissionDic[tar].Remove(mat);
            }
        }
    }
}

public interface IItemRenderer
{
    Action<object, string> OnItemMessage { set; }
    int Index { get; set; }
    object Data { get; set; }
    void onDelete();
}

public static class JsonUtils
{

    public static float ReadFloat(this JsonData jd, string key, float defaultValue = 0)
    {
        float res;
        if (jd.Keys.Contains(key) && jd[key] != null && float.TryParse(jd[key].ToString(), out res))
            return res;
        else
            return defaultValue;
    }
    public static int ReadInt(this JsonData jd, string key, int defaultValue = 0)
    {
        int res;
        if (jd.Keys.Contains(key) && jd[key] != null && int.TryParse(jd[key].ToString(), out res))
            return res;
        else
            return defaultValue;
    }

    public static bool ReadBool(this JsonData jd, string key)
    {
        if(jd.Keys.Contains(key) && jd[key] != null)
            return jd[key].ToString() == "true" || jd[key].ToString() == "是";
        else
            return false;
    }

    public static string ReadString(this JsonData jd, string key, string defaultValue = null)
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return jd[key].ToString();
        else
            return defaultValue;
    }

    public static Dictionary<T1, T2> ReadDic<T1, T2>(this JsonData jd, string key, Dictionary<T1, T2> defaultValue = null)
    {

        if (jd.Keys.Contains(key) && jd[key] != null)
            return JsonMapper.ToObject<Dictionary<T1, T2>>(jd[key].ToJson());
        else
            return defaultValue;
    }

    public static JsonData WriteJsonData<T>(this JsonData jd, string key, T value)
    {
        if (typeof(T) == typeof(int))
            jd[key] = int.Parse(value.ToString());
        else if (typeof(T) == typeof(float))
            jd[key] = float.Parse(value.ToString());
        else if (typeof(T) == typeof(string))
            jd[key] = value.ToString();
        else if (typeof(T).IsAssignableFrom(typeof(IJsonData)))
            jd[key] = JsonMapper.ToJson((value as IJsonData).ToJsonData());
        else if (typeof(T) == typeof(JsonData))
            jd[key] = value as JsonData;
        else
            jd[key] = value.ToString();
        return jd;
    }

    public static void DicChange(Dictionary<string, float> src, Dictionary<string, float> delta, bool isAdd)
    {
        foreach (var key in delta.Keys)
        {
            if (!src.ContainsKey(key))
                src.Add(key, 0);
            float change = isAdd ? delta[key] : -delta[key];
            src[key] = src[key] + change;
        }
    }

    public static JsonData ToJsonData<T2>(this Dictionary<string, T2> dic)
    {
        JsonData jd = new JsonData();
        foreach (var key in dic.Keys)
        {
            jd.WriteJsonData(key, dic[key]);
        }
        return jd;
    }

    public static Vector3 ReadVec3(this JsonData jd, string key, Vector3 defaultValue = default(Vector3))
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return JsonToVec3(jd[key]);
        else
            return defaultValue;
    }

    public static T ReadEnum<T>(this JsonData jd, string key)
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return (T)Enum.Parse(typeof(T), jd[key].ToString(), true);
        else
            return default(T);
    }


    public static Vector3 JsonToVec3(JsonData jd)
    {
        return StringToVector3(jd.ToString());
    }

    public static Vector3 StringToVector3(string str)
    {
        string[] floatArr = str.Split(new Char[] { ',' });
        return new Vector3(float.Parse(floatArr[0]), float.Parse(floatArr[1]), float.Parse(floatArr[2]));
    }

    public static string vecToStr(Vector3 vec)
    {
        return vec.x.ToString("f3") + "," + vec.y.ToString("f3") + "," + vec.z.ToString("f3");
    }

    public static JsonData ToJsonData(this Rect rect)
    {
        JsonData jd = new JsonData();
        jd["x"] = rect.x;
        jd["y"] = rect.y;
        jd["width"] = rect.width;
        jd["height"] = rect.height;
        return jd;
    }

    public static JsonData ToJsonData(object obj)
    {
        return JsonMapper.ToObject(JsonMapper.ToJson(obj));
    }

    public static T ToItemVO<T>(this JsonData jd) where T : new()
    {
        //判断T是否是IJsonData, 若是则用ReadJsonData解析
        T tar = new T();
        bool tt = tar is IJsonData;
        if (tar is IJsonData)
        {
            (tar as IJsonData).ReadJsonData(jd);
            return tar;
        }
        else
            return JsonMapper.ToObject<T>(JsonMapper.ToJson(jd));
    }

    public static List<T> ToItemVOList<T>(this JsonData jd) where T: new()
    {
        List<T> list = new List<T>();
        for (int i = 0, length = jd.Count; i < length; i++)
        {
            list.Add(ToItemVO<T>(jd[i]));
        }
        return list;
    }

    public static List<Vector3> ToVec3List(this JsonData jd)
    {
        List<Vector3> list = new List<Vector3>();
        for (int i = 0, length = jd.Count; i < length; i++)
        {
            list.Add(JsonToVec3(jd[i]));
        }
        return list;
    }

    public static JsonData ToJsonDataList<T>(this ICollection<T> list) where T : IJsonData
    {
        JsonData jd = ToJsonData(new List<int>());
        foreach (var item in list)
        {
            jd.Add((item as IJsonData).ToJsonData());
        }
        return jd;
    }

    public static JsonData ToJsonDataDic<T1, T2>(this Dictionary<T1, T2> dic) where T2 : IJsonData
    {
        JsonData jd = new JsonData();
        foreach (var pair in dic)
        {
            string key = pair.Key.ToString();
            JsonData value = pair.Value.ToJsonData();
            jd[key] = value;
        }
        return jd;
    }

    //TODO 测试中
    public static bool SetJsonProp<T>(object tar, JsonData jd, string propName, string jsonKey, Func<JsonData, T> dataProcess = null)
    {
        if (jd.Keys.Contains(jsonKey))
        {
            JsonData valueJD = jd[jsonKey];
            PropertyInfo prop = null;
            foreach (PropertyInfo p in tar.GetType().GetProperties())
            {
                if (p.Name == propName)
                {
                    prop = p;
                    break;
                }
            }
            if (prop != null)
            {
                if (dataProcess == null)
                {
                    if (typeof(T) == typeof(string))
                        prop.SetValue(tar, JsonMapper.ToJson(valueJD), null);
                    else if (typeof(T) == typeof(int))
                        prop.SetValue(tar, int.Parse(JsonMapper.ToJson(valueJD)), null);
                    else if (typeof(T) == typeof(float))
                        prop.SetValue(tar, float.Parse(JsonMapper.ToJson(valueJD)), null);
                    else if (typeof(T) == typeof(Vector3))
                        prop.SetValue(tar, JsonToVec3(valueJD), null);
                    else
                    {
                        //Debug.Log("SetJsonProp failed! no fitful default dataProcess(string, int, float, Vector3 ) ");
                        return false;
                    }
                }
                else
                    prop.SetValue(tar, dataProcess(valueJD), null);
                return true;
            }
            else
            {
                string tarName = tar is MonoBehaviour ? (tar as MonoBehaviour).name : tar.ToString();
                //Debug.Log("SetJsonProp failed! can not find propName: [" + propName + "] in tar" + tarName);
                return false;
            }
        }
        else
        {
            //Debug.Log("SetJsonProp failed! can not find jsonKey: [" + jsonKey + "] in JsonData : \r\n"+JsonMapper.ToJson(jd));
            return false;
        }

    }
}
public interface IJsonData
{
    JsonData ToJsonData();
    IJsonData ReadJsonData(JsonData jd);
}

#region PerspectiveTransform 变换矩阵
public class PerspectiveTransform
{
    float a11;
    float a12;
    float a13;
    float a21;
    float a22;
    float a23;
    float a31;
    float a32;
    float a33;
    public PerspectiveTransform(float inA11, float inA21,
                                       float inA31, float inA12,
                                       float inA22, float inA32,
                                       float inA13, float inA23,
                                       float inA33)
    {
        a11 = inA11;
        a12 = inA12;
        a13 = inA13;

        a21 = inA21;
        a22 = inA22;
        a23 = inA23;

        a31 = inA31;
        a32 = inA32;
        a33 = inA33;
    }

    public static PerspectiveTransform quadrilateralToQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, float x0p, float y0p, float x1p, float y1p, float x2p, float y2p, float x3p, float y3p)
    {
        PerspectiveTransform qToS = quadrilateralToSquare(x0, y0, x1, y1, x2, y2, x3, y3);
        PerspectiveTransform sToQ = squareToQuadrilateral(x0p, y0p, x1p, y1p, x2p, y2p, x3p, y3p);
        return sToQ.times(qToS);
    }

    static PerspectiveTransform squareToQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
    {
        float dx3 = x0 - x1 + x2 - x3;
        float dy3 = y0 - y1 + y2 - y3;
        if (dx3 == 0.0f && dy3 == 0.0f)
        {
            return new PerspectiveTransform(x1 - x0, x2 - x1, x0, y1 - y0, y2 - y1, y0, 0.0f, 0.0f, 1.0f);
        }
        else
        {
            float dx1 = x1 - x2;
            float dx2 = x3 - x2;
            float dy1 = y1 - y2;
            float dy2 = y3 - y2;
            float denominator = dx1 * dy2 - dx2 * dy1;
            float a13 = (dx3 * dy2 - dx2 * dy3) / denominator;
            float a23 = (dx1 * dy3 - dx3 * dy1) / denominator;
            return new PerspectiveTransform(x1 - x0 + a13 * x1, x3 - x0 + a23 * x3, x0, y1 - y0 + a13 * y1, y3 - y0 + a23 * y3, y0, a13, a23, 1.0f);
        }
    }

    static PerspectiveTransform quadrilateralToSquare(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
    {
        return squareToQuadrilateral(x0, y0, x1, y1, x2, y2, x3, y3).buildAdjoint();
    }

    PerspectiveTransform buildAdjoint()
    {
        return new PerspectiveTransform(a22 * a33 - a23 * a32, a23 * a31 - a21 * a33, a21 * a32
                               - a22 * a31, a13 * a32 - a12 * a33, a11 * a33 - a13 * a31, a12 * a31 - a11 * a32, a12 * a23 - a13 * a22,
                               a13 * a21 - a11 * a23, a11 * a22 - a12 * a21);
    }

    PerspectiveTransform times(PerspectiveTransform other)
    {
        return new PerspectiveTransform(a11 * other.a11 + a21 * other.a12 + a31 * other.a13,
                               a11 * other.a21 + a21 * other.a22 + a31 * other.a23, a11 * other.a31 + a21 * other.a32 + a31
                               * other.a33, a12 * other.a11 + a22 * other.a12 + a32 * other.a13, a12 * other.a21 + a22
                               * other.a22 + a32 * other.a23, a12 * other.a31 + a22 * other.a32 + a32 * other.a33, a13
                               * other.a11 + a23 * other.a12 + a33 * other.a13, a13 * other.a21 + a23 * other.a22 + a33
                               * other.a23, a13 * other.a31 + a23 * other.a32 + a33 * other.a33);
    }

    public void transformPoints(List<float> points)
    {
        int max = points.Count;
        for (int i = 0; i < max; i += 2)
        {
            float x = points[i];
            float y = points[i + 1];
            float denominator = a13 * x + a23 * y + a33;
            points[i] = (a11 * x + a21 * y + a31) / denominator;
            points[i + 1] = (a12 * x + a22 * y + a32) / denominator;
        }
    }


}

#endregion