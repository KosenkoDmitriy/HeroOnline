using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageBox  {

    public static bool isShow = false;
    public static Dictionary<int, GameObject> dialogSet = new Dictionary<int, GameObject>();


    public static void ShowDialog(string message, UINoticeManager.NoticeType type)
    {
        if (!dialogSet.ContainsKey(dialogSet.Count))
        {

            CloseDialog();

            GameObject prefab = Resources.Load("Prefabs/UI/UINoticeRoot") as GameObject;
            GameObject dialog = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            UINoticeManager notice = dialog.GetComponent<UINoticeManager>();
            notice.SetNotice(type, message, dialogSet.Count);
            dialogSet[dialogSet.Count] = dialog;
        }
       // isShow = true;
    }

    public static void CloseDialog()
    {
        foreach (GameObject obj in dialogSet.Values)
        {
            if (obj == null)
            {
                dialogSet.Clear();
                return;
            }
            // if (obj.GetComponent<UINoticeManager>().noticetype == UINoticeManager.NoticeType.Waiting)
            //     GameObject.Destroy(obj, 1f);
            // else
            GameObject.Destroy(obj, 0.1f);
        }

        dialogSet.Clear();
        isShow = false;
    }

    public static void CloseDialog(int id)
    {
        if (dialogSet.ContainsKey(id))
        {
            GameObject.Destroy(dialogSet[id], 0.3f);

            dialogSet.Remove(id);
            if (dialogSet.Count <= 0)
                isShow = false;
        }
    }

}
