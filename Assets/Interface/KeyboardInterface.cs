using UnityEngine;

public class KeyboardInterface : MonoBehaviour {

    CursorInterface cursorInterface;

    public bool renderGrid{ get; private set; }

    private Modifier modifier;

    // Use this for initialization
	void Start ()
    {
        this.renderGrid = true;
        this.modifier = Modifier.NONE;
        cursorInterface = this.gameObject.GetComponent<CursorInterface>();
        updateModifierText();
        updateSelectionTypeText(SelectType.NONE);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
        if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift) && 
            this.modifier == Modifier.SHIFT)
        {
            this.modifier = Modifier.NONE;
            updateModifierText();
        }

        if(Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl) &&
            this.modifier == Modifier.CTRL)
        {
            this.modifier = Modifier.NONE;
            updateModifierText();
        }

        if(Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt) &&
            this.modifier == Modifier.CTRL)
        {
            this.modifier = Modifier.NONE;
            updateModifierText();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            this.modifier = Modifier.SHIFT;
            updateModifierText();
        }

        if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            this.modifier = Modifier.CTRL;
            updateModifierText();
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            this.modifier = Modifier.ALT;
            updateModifierText();
        }

        if(Input.GetKeyDown(KeyCode.F1))
        {
            renderGrid = !renderGrid;
        }

        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            updateCursorSelectionType(SelectType.NONE);
            updateSelectionTypeText(SelectType.NONE);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            updateCursorSelectionType(SelectType.POINT);
            updateSelectionTypeText(SelectType.POINT);
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            updateCursorSelectionType(SelectType.SQUARE);
            updateSelectionTypeText(SelectType.SQUARE);
        }
	}

    private void updateModifierText()
    {
        GameObject.Find("ModifierType").GetComponent<GUITextField>().setText(this.modifier.ToString());
    }

    private void updateSelectionTypeText(SelectType s)
    {
        GameObject.Find("SelectionType").GetComponent<GUITextField>().setText(s.ToString());
    }

    public Modifier getModifier()
    {
        return this.modifier;
    }

    private void updateCursorSelectionType(SelectType s)
    {
        this.cursorInterface.setSelectionState(s);
    }
}
