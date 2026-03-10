using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;

public class TestPeoplePortrait : MonoBehaviour
{
    public Toggle toggle_male;
    public Toggle toggle_female;
    public Button btn;
    Gender gender = Gender.Female;
    public Portrait portrait;
    PeopleData PeopleData;
    // Start is called before the first frame update
    void Start()
    {
        toggle_female.onValueChanged.AddListener((x) =>
        {
            Female(x);
        });
        toggle_male.onValueChanged.AddListener((x) =>
        {
            Female(!x);
        });
        toggle_female.onValueChanged.Invoke(true);

        btn.onClick.AddListener(() =>
        {
            RdmGenerateNew(gender);
        });

        btn.onClick.Invoke();
    }

    void Female(bool isFemale)
    {
        if (isFemale)
            gender = Gender.Female;
        else
            gender = Gender.Male;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 生成新的
    /// </summary>
    void RdmGenerateNew(Gender gender)
    { 

           PeopleData = new PeopleData();
         PeopleData.gender = (int)gender;
        PeopleData p = PeopleData;
        RoleManager.Instance.RdmFace(p);
        portrait.Refresh(p);
    }
}
