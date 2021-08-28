#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using System.Collections.Generic;


public class Dustbin : MonoBehaviour, IBindable, IPointable
{
    public bool IsPointed { get; set; }
    [SerializeField]
    private float pointOffset;
    public float PointOffset { get { return pointOffset; } }

    private struct DisposableKey
    {
        public IDustbinDisposable disposable;
        public int instance_id;

        public DisposableKey(IDustbinDisposable _disposable, int _instance_id)
        {
            this.instance_id = _instance_id;
            this.disposable = _disposable;
        }
    }

    public List<IBindable> BindedObjects { get; set; }
    public bool IsBinded { get { return BindedObjects.Count != 0; } }
    [SerializeField]
    private StandardAnimation PopUpAnimData;

    private float text_offset;

    private List<DisposableKey> Disposables;
    private AnimatedText PopUpText;
    private void Awake()
    {
        BindedObjects = new List<IBindable>();
        PopUpText = AnimatedText.UIText(PopUpAnimData, new Vector2(200, 70), Color.white, 50);
        Disposables = new List<DisposableKey>();
    }
    private void Start()
    {
        text_offset = GetComponentInChildren<Collider2D>().bounds.extents.y;
        GameManager.GetBindManager().RegisterBindable(this, GameConstants.TouchIDs.DUSTBIN);
        GameManager.GetPointManager().RegisterPointable(this);
    }
    private void Update()
    {
        GameLoop.Update(PopUpText);
    }

    private void OnDestroy()
    {
        GameManager.GetBindManager().UnregisterBindable(this);
        GameManager.GetPointManager().UnregisterPointable(this);
    }

    private void OnTriggerEnter2D(Collider2D colliderInfo)
    {
        if (colliderInfo.GetComponent<IDustbinDisposable>() != null)
            Disposables.Add(new DisposableKey(colliderInfo.GetComponent<IDustbinDisposable>(), colliderInfo.GetInstanceID()));
#if DEBUG_MODE
        else
        {
            GameManager.GetLogManager().Log(string.Format("{0} is not disposable", colliderInfo.name));
        }
#endif
    }
    private void OnTriggerStay2D(Collider2D colliderInfo)
    {
        int disposable_count = Disposables.Count;
        for (int i = 0; i < disposable_count; i++)
        {
            DisposableKey disposablekey = Disposables[i];
            IBindable bindable = (disposablekey.disposable as MonoBehaviour).GetComponent<IBindable>();
            if (((bindable == null) && !disposablekey.disposable.IsDisposed) || ((bindable != null) && !bindable.IsBinded))
            {
                DisposeIt(disposablekey.disposable);
                Disposables.Remove(disposablekey);
                --disposable_count;
#if DEBUG_MODE
                GameManager.GetLogManager().Log((disposablekey.disposable as MonoBehaviour).name + " is disposed");
#endif
            }
        }
    }
    private void DisposeIt(IDustbinDisposable disposable)
    {
        disposable.Dispose();
        StaticMemory.DisposedCount++;
        PopUpText.PopUp("Yeah", GameManager.GetCamera().WorldToScreenPoint(transform.position + Vector3.up * text_offset), Vector3.zero, Quaternion.identity);
    }
    private void OnTriggerExit2D(Collider2D colliderInfo)
    {
        if (Disposables.Count != 0)
            Disposables.Clear();
    }
}
