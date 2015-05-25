using UnityEngine;
using System.Collections;

public class SliderTips : MonoBehaviour {
    public SingleButtonCallBack ThumbBtn;
    public UISlider Slider;
    public UILabel Lable;
    private bool IsShow;
    private Transform ThumbTra;
    public TipsType type;
	void Awake()
    {
        ThumbBtn.SetPressCallBack(OnPress);
        //ThumbBtn.SetDragCallback(OnValueChange);
       
        Slider.onValueChange+=OnValueChange;
        ThumbTra=ThumbBtn.transform;
    }

    void OnPress(bool Ispress)
    {

        Vector3 Fpos=new Vector3(ThumbTra.localPosition.x,45,-10);
        Vector3 Tpos=new Vector3(ThumbTra.localPosition.x,48,-10);
        if(Ispress)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_System_Pop");
            IsShow=true;
            SetString();
            TweenPosition.Begin(gameObject,0.17f,Fpos,Tpos);
            TweenAlpha.Begin(gameObject,0.17f,0,1,null);
          
        }
        else
        {
            IsShow=false;
            TweenPosition.Begin(gameObject,0.17f,Tpos,Fpos);
            TweenAlpha.Begin(gameObject,0.17f,1,0,null);
        }

    }

    void SetString()
    {
        transform.localPosition= new Vector3(ThumbTra.localPosition.x,48,-10);
        string str;
        if(type==TipsType.Percentage)
        {
          str = ((int)(Slider.sliderValue* 100)) + "%";
       
        }
        else
        {
            str=GetStep().ToString();
        }
        Lable.SetText(str);
    }

    int GetStep()
    {
        float TotalStep= Slider.numberOfSteps-1;

		return Mathf.RoundToInt(Slider.sliderValue * TotalStep) + 1;
    }

   void  OnValueChange(float value)
    {
        if(IsShow)
        {
         SetString();
        }
    }
   public enum TipsType
    {
        Percentage,
        Step,
    }
}
