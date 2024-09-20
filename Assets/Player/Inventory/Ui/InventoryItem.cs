using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Ui reprezentation of item in inventory
public class InventoryItem
{
    public Item item;
    public int x=0, y=0;
    public int width, height;
    public Image BackGround;
    public Image icon;
    public List<UiInventorySlot> slotsTaken = new List<UiInventorySlot>();

    int tileSize;

    bool flip = false;

    public InventoryItem(Item newItem)
    {
        item = newItem;
        width = item.UIwidth;
        height = item.UIheight;
    }

    public void SetUpItem(Vector2Int pozition, List<UiInventorySlot> itemSlots)
    {
        x = pozition.x;
        y = pozition.y;
        
        slotsTaken = itemSlots;
    }
    public void SetUpItemVisuals(Image imageTm, int tileSize)
    {
        BackGround = imageTm;
        if (flip)
        {
            BackGround.rectTransform.sizeDelta = new Vector2(height * tileSize, width * tileSize);
            BackGround.rectTransform.Rotate(new Vector3(0, 0, 1), -90f);
        }
        else
            BackGround.rectTransform.sizeDelta = new Vector2(width * tileSize, height * tileSize);
        icon = BackGround.transform.GetChild(0).GetComponent<Image>();
        icon.sprite = item.icon;
        BackGround.transform.SetAsLastSibling();
        BackGround.GetComponent<InventoryItemBehaviour>().uiInventoryItem = this;
        this.tileSize = tileSize;
        UpdateUiPozition();
    }
    public void CleerItemVisuals()
    {
        GameObject.Destroy(BackGround.gameObject);
        BackGround = null;
        //tileSize = 0;
    }
    ~ InventoryItem()
    {
        //Debug.Log("IVE BEEN DESTROYED");
    }

    public void ChengePozition(List<UiInventorySlot> newSlotsTaken)
    {
        slotsTaken.Clear();
        slotsTaken = newSlotsTaken;
        x = newSlotsTaken[0].x; 
        y = newSlotsTaken[0].y;
        UpdateUiPozition();
    } 

    public void UpdateUiPozition()
    {
        this.TakeSlots();
        //offset when midle of Item image is in midle of tile
        Vector2 offset = Vector2.zero;
        if (width % 2 != 0)
        {
            offset.x = tileSize/2;
        }
        if (height % 2 != 0)
        {
            offset.y = tileSize/2;
        }
        BackGround.rectTransform.localPosition = new Vector2((x * tileSize)+(width / 2 * tileSize)+ offset.x, (-y * tileSize)+(-height/2*tileSize)- offset.y);
    }

    public void TakeSlots()
    {
        foreach (UiInventorySlot slot in slotsTaken)
        {
            slot.taken = true;
        }
    }

    public void FreeSlots()
    {
        foreach (UiInventorySlot slot in slotsTaken)
        {
            slot.taken = false;
        }
    }

    public void Flip()
    {
        Rotate();
        if (flip)
        {
            if(BackGround!=null)
                BackGround.rectTransform.Rotate(new Vector3(0,0,1), 90f);
            flip = false;
        }
        else
        {
            if (BackGround != null)
                BackGround.rectTransform.Rotate(new Vector3(0, 0, 1), -90f);
            flip = true;
        }
            
    }

    void Rotate()
    {
        int tmp = width;
        width = height;
        height = tmp;
    }

}
