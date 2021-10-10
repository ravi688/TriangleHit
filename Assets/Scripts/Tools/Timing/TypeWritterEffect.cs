using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class TypeWritter : IManualUpdatable
{

    public bool IsRunning { get { return _is_writting; } }
    public int speed = 700;                      //700 character per minute
    public System.Action OnEnd; 
    private string _str_to_write;
    private bool _is_writting;
    private int _num_character_written;
    private float _time_step;
    private Timer _writting_timer;
    private StringBuilder _writting_string;
    private Text _text;
    private TypeWritter() { _is_writting = false; }
    public TypeWritter(Text _text)
        : this()
    {
        this._text = _text;
    }
    public void Write(string str)
    {
        _str_to_write = str;
        _time_step = (float)60 / (float)speed;
        _writting_timer = null;
        _writting_timer = new Timer(0, _time_step *( _str_to_write.Length - 1), _time_step);
        _writting_timer.AddListner(
            delegate
            {
                _writting_string.Append(_str_to_write[_num_character_written++]);
                _text.text = _writting_string.ToString();            
            },
        OnTimer.Update);
        _writting_timer.AddListner(
            delegate
            {
                _is_writting = false;
                if (OnEnd != null)
                {
                    OnEnd();
                }
            },
            OnTimer.End);
        _writting_timer.AddListner(
            delegate
            {
                _num_character_written = 0;
                _is_writting = true;
                _writting_string.Length = 0;
            },
            OnTimer.Start);
        if (_writting_string == null)
            _writting_string = new StringBuilder();
        _writting_timer.Start();
    }
    public void Update()
    {
        _writting_timer.Update();
    }
}
