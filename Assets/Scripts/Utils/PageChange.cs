using UnityEngine;
using System.Collections;

public class PageChange : MonoBehaviour {

    public string PageToChange;
    public KeyCode codetoChange;
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(codetoChange))
        {
            Application.LoadLevel(PageToChange);
        }
	}

    public void ChangePage()
    {
        Application.LoadLevel(PageToChange);
    }
}
