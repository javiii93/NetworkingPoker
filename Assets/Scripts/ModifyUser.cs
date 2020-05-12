using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifyUser : MonoBehaviour
{
    #region Public Fields
    public Text name, surname, coins;
    string userData;
    //public Launcher launcher = new Launcher();
//   Launcher launcher;
   /* [SerializeField]
    public Launcher launcher;*/
    #endregion

        void Awake()
    {
      //  Debug.Log(Launcher.data);

    }
    // Start is called before the first frame update
    void Start()
    {
        //launcher = GameObject.Find("Launcher");
        //launcher = FindObjectOfType<launcher>();
        // launcher = new Launcher();
        //  string all= Launcher.data;
        // Debug.Log(Launcher.all);
        // userData = Launcher.launcher.data;
       // Debug.Log(Launcher.data);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
