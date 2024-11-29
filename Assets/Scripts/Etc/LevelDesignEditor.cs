using UnityEngine;
using UnityEditor;

// 레벨 디자인의 내용을 그래프로 표현 
[CustomEditor(typeof(LevelDesign))]
public class LevelDesignEditor : Editor
{
    LevelDesign design = null;

    [SerializeField] Color Color_HP = Color.red;
    [SerializeField] Color Color_EXP = Color.blue;
    [SerializeField] Color Color_MaxEXP = Color.white;
    [SerializeField] Color Color_ATK = Color.green;
    [SerializeField] Color Color_Money = Color.yellow;


    public override void OnInspectorGUI()
    {
        design = (LevelDesign)target;
        EditorGUILayout.LabelField("Level Design", EditorStyles.boldLabel);

        LevelData data = design.mLevelData;
        StageData s_data = design.mStageData;

        DrawGraph(data, s_data);
        EditorGUILayout.Space();
        DrawDefaultInspector();
    }

    private void DrawGraph(LevelData data, StageData s_Data)
    {
        EditorGUILayout.LabelField("[ ==== Player Growth Level Graph ==== ]", EditorStyles.boldLabel);

        //그래프 영역 생성 
        Rect rect = GUILayoutUtility.GetRect(200,100);
        //실제 그리기
        Handles.DrawSolidRectangleWithOutline(rect, Color.black, Color.white);

        //그래프 그리기
        Vector3[] curvePoint_HP = DesignGraph(rect, data.C_HP);
        Handles.color = Color_HP;
        Handles.DrawAAPolyLine(3, curvePoint_HP);  // 선의 굵기, 표현할 Vector 배열 값 

        Vector3[] curvePoint_EXP = DesignGraph(rect, data.C_EXP);
        Handles.color = Color_EXP;
        Handles.DrawAAPolyLine(3, curvePoint_EXP);  // 선의 굵기, 표현할 Vector 배열 값 

        Vector3[] curvePoint_ATK = DesignGraph(rect, data.C_ATK);
        Handles.color = Color_ATK;
        Handles.DrawAAPolyLine(3, curvePoint_ATK);  // 선의 굵기, 표현할 Vector 배열 값 

        Vector3[] curvePoint_MaxEXP = DesignGraph(rect, data.C_MAXEXP);
        Handles.color = Color_MaxEXP;
        Handles.DrawAAPolyLine(3, curvePoint_MaxEXP);  // 선의 굵기, 표현할 Vector 배열 값 

        Vector3[] curvePoint_Money = DesignGraph(rect, data.C_MONEY);
        Handles.color = Color_Money;
        Handles.DrawAAPolyLine(3, curvePoint_Money);  // 선의 굵기, 표현할 Vector 배열 값 

        EditorGUILayout.Space(10);
        //값 찍기 // 시작값 - 현재레벨 - 레벨 별 증가 값
        GetColorGUI("HP", StringMethod.ToCurrencyString(Utils.CalculatedValue(data.base_HP,design.currentLevel, data.C_HP)), Color_HP);
        GetColorGUI("EXP", StringMethod.ToCurrencyString(Utils.CalculatedValue(data.base_EXP,design.currentLevel, data.C_EXP)), Color_EXP);
        GetColorGUI("MaxEXP", StringMethod.ToCurrencyString(Utils.CalculatedValue(data.base_MAXEXP,design.currentLevel, data.C_MAXEXP)), Color_MaxEXP);
        GetColorGUI("ATK", StringMethod.ToCurrencyString(Utils.CalculatedValue(data.base_ATK,design.currentLevel, data.C_ATK)), Color_ATK);
        GetColorGUI("Money", StringMethod.ToCurrencyString(Utils.CalculatedValue(data.base_MONEY,design.currentLevel, data.C_MONEY)), Color_Money);
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("[ ==== Stage Monster Growth Data ==== ]", EditorStyles.boldLabel);

        GetColorGUI("HP", StringMethod.ToCurrencyString(Utils.CalculatedValue(s_Data.base_HP,design.currentStage, s_Data.M_HP)), Color_HP);      
        GetColorGUI("ATK", StringMethod.ToCurrencyString(Utils.CalculatedValue(s_Data.base_ATK, design.currentStage, s_Data.M_ATK)), Color_ATK);
        GetColorGUI("Money", StringMethod.ToCurrencyString(Utils.CalculatedValue(s_Data.base_MONEY, design.currentStage, s_Data.M_MONEY)), Color_Money);
       
    }
    //에디터에서는 <color> 필드가 안먹어서 아래와 같이 적용함
    void GetColorGUI(string baseTemp, string dataTemp, Color color)
    {
        GUIStyle colorLabel = new GUIStyle(EditorStyles.label);
        colorLabel.normal.textColor = color;
        EditorGUILayout.LabelField($"{baseTemp} : {dataTemp}",colorLabel);
    }

    private Vector3[] DesignGraph(Rect rect, float data)
    {
        Vector3[] curvePoint = new Vector3[100];
        for(int i=0;i<100;i++)
        {
            float t = i / 90f;
            float curveValue = Mathf.Pow(t,data);
            curvePoint[i] = new Vector3(
                rect.x + t * rect.width,                                //x
                rect.y + rect.height - curveValue * rect.height,        //y 지수에 따른 값 변화
                0f
            );
        }
        return curvePoint;
    }

}


