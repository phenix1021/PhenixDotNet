using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Phenix.Unity.UI
{
    using Sprite = UnityEngine.Sprite;

    public enum FlipMode
    {
        RightToLeft,
        LeftToRight
    }

    [System.Serializable]
    public class UnityEventPageClicked : UnityEvent<int/*点击的页面号*/> { }

    [System.Serializable]
    public class UnityEventPageShow : UnityEvent<int/*页面号*/, Image, Image> { }

    [AddComponentMenu("Phenix/UI/FlipPage")]
    public class FlipPage : MonoBehaviour
    {
        public Canvas canvas;                
        public RectTransform bookPanel;                
        public List<Sprite> papers = new List<Sprite>();

        public bool interactable = true;
        public bool enableShadowEffect = true;
                
        [Tooltip("当前页号（一页包括左右两张纸）")]
        [SerializeField]
        int _curPage = 0;

        public Image clippingPlane;
        public Image nextPageClip;
        public Image shadow;
        public Image shadowLTR;
        public Image leftOnFlip;
        public Image left;
        public Image rightOnFlip;
        public Image right;
        public Image leftHotSpot;
        public Image rightHotSpot;

        Vector2 _oriPosClippingPlane;
        Quaternion _oriRotClippingPlane;
        Vector2 _oriPosNextPageClip;
        Quaternion _oriRotNextPageClip;
        Vector2 _oriPosLeftOnFlip;
        Quaternion _oriRotLeftOnFlip;
        Vector2 _oriPosRightOnFlip;
        Quaternion _oriRotRightOnFlip;

        float _radius1, _radius2;

        bool _autoFlipReleased = true;  // 自动翻页是否已释放（end drag）
        bool _isAutoFlipping;           // 是否自动翻页
        float _elapseAutoFlipTime;      // 已经过的自动翻页时长（秒）
        float _autoFlipTime;            // 自动翻页时长（秒）
        Vector3 _autoFlipSrcPos;        // 自动翻页起始点（begin drag的位置）
        Vector3 _autoFlipDstPos;        // 自动翻页目标点（end drag的位置）

        //Spine Bottom
        Vector3 _sb;
        //Spine Top
        Vector3 _st;
        //corner of the page
        Vector3 _c;
        //Edge Bottom Right
        Vector3 _ebr;
        //Edge Bottom Left
        Vector3 _ebl;
        //follow point 
        Vector3 _f;

        // 是否正在翻页
        bool _pageFlipping = false;

        //current flip mode
        FlipMode _mode;

        Coroutine _currentCoroutine;

        public UnityEvent onPageFlipped;                          // 翻页完成
        //public UnityEventPageClicked onPageClicked;               // 点击页面
        public UnityEventPageShow onPageShow;                     // 页面显示

        int CurPaper { get { return _curPage * 2 + 1; } }
        public int CurPage { get { return _curPage; } set { _curPage = value; } }

        public Vector3 EndBottomLeft { get { return _ebl; } }
        public Vector3 EndBottomRight { get { return _ebr; } }
        public float Height { get { return bookPanel.rect.height; } }

        // papers集合是否符合要求
        bool CheckPapers()
        {            
            return (papers.Count > 0 && papers.Count % 2 == 0);
        }

        // 页号是否合理
        bool CheckPageNumber(int pageNumber)
        {
            return (CheckPapers() && pageNumber >= 0 && pageNumber <= (papers.Count / 2 - 1));
        }

        // 转到指定页号
        public bool TurnToPage(int pageNumber)
        {
            if (CheckPageNumber(pageNumber) == false)
            {
                return false;
            }

            if (_pageFlipping)
            {
                return false;
            }

            _curPage = pageNumber;
            UpdatePage();
            return true;
        }

        public void Reset()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }            

            leftOnFlip.gameObject.SetActive(false);
            rightOnFlip.gameObject.SetActive(false);

            _curPage = 0;
            left.sprite = papers[_curPage * 2];
            right.sprite = papers[_curPage * 2 + 1];

            _isAutoFlipping = false;
            _pageFlipping = false;
            _autoFlipReleased = true;
            _elapseAutoFlipTime = 0;

            clippingPlane.transform.position = _oriPosClippingPlane;
            clippingPlane.transform.rotation = _oriRotClippingPlane;
            nextPageClip.transform.position = _oriPosNextPageClip;
            nextPageClip.transform.rotation = _oriRotNextPageClip;
            leftOnFlip.transform.position = _oriPosLeftOnFlip;
            leftOnFlip.transform.rotation = _oriRotLeftOnFlip;
            rightOnFlip.transform.position = _oriPosRightOnFlip;
            rightOnFlip.transform.rotation = _oriRotRightOnFlip;
        }

        void Start()
        {
            if (CheckPapers() == false)
            {
                Debug.LogWarning("papers count must be greater than zero, and an even number.");
                return;
            }

            float pageWidth = (bookPanel.rect.width - 1) / 2;
            float pageHeight = bookPanel.rect.height;

            leftOnFlip.gameObject.SetActive(false);
            rightOnFlip.gameObject.SetActive(false);
            
            _curPage = Mathf.Clamp(_curPage, 0, papers.Count / 2 - 1);            

            UpdatePage();

            _oriPosClippingPlane = clippingPlane.transform.position;
            _oriRotClippingPlane = clippingPlane.transform.rotation;
            _oriPosNextPageClip = nextPageClip.transform.position;
            _oriRotNextPageClip = nextPageClip.transform.rotation;
            _oriPosLeftOnFlip = leftOnFlip.transform.position;
            _oriRotLeftOnFlip = leftOnFlip.transform.rotation;
            _oriPosRightOnFlip = rightOnFlip.transform.position;
            _oriRotRightOnFlip = rightOnFlip.transform.rotation;

            _sb = new Vector3(0, -bookPanel.rect.height / 2);
            _ebr = new Vector3(bookPanel.rect.width / 2, -bookPanel.rect.height / 2);
            _ebl = new Vector3(-bookPanel.rect.width / 2, -bookPanel.rect.height / 2);

            _st = new Vector3(0, bookPanel.rect.height / 2);
            _radius1 = Vector2.Distance(_sb, _ebr);
            _radius2 = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);

            float shadowPageHeight = pageWidth / 2 + _radius2;
            shadow.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            shadow.rectTransform.pivot = new Vector2(1, (pageWidth / 2) / shadowPageHeight);
            shadowLTR.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            shadowLTR.rectTransform.pivot = new Vector2(0, (pageWidth / 2) / shadowPageHeight);

            clippingPlane.rectTransform.sizeDelta = new Vector2(pageWidth * 2, pageHeight + pageWidth * 2);
            nextPageClip.rectTransform.sizeDelta = new Vector2(pageWidth, pageHeight + pageWidth * 0.6f);

            EventTrigger.Entry dragRelease = new EventTrigger.Entry();
            dragRelease.eventID = EventTriggerType.EndDrag;
            dragRelease.callback.AddListener(new UnityAction<BaseEventData>((x) => { OnMouseRelease(); }));

            EventTrigger.Entry dragLeft = new EventTrigger.Entry();
            dragLeft.eventID = EventTriggerType.BeginDrag;
            dragLeft.callback.AddListener(new UnityAction<BaseEventData>((x) => { OnMouseDragLeftPage(); }));
            leftHotSpot.GetComponent<EventTrigger>().triggers.Add(dragLeft);
            leftHotSpot.GetComponent<EventTrigger>().triggers.Add(dragRelease);

            EventTrigger.Entry dragRight = new EventTrigger.Entry();
            dragRight.eventID = EventTriggerType.BeginDrag;
            dragRight.callback.AddListener(new UnityAction<BaseEventData>((x) => { OnMouseDragRightPage(); }));
            rightHotSpot.GetComponent<EventTrigger>().triggers.Add(dragRight);
            rightHotSpot.GetComponent<EventTrigger>().triggers.Add(dragRelease);

            /*EventTrigger.Entry leftClick = new EventTrigger.Entry();
            leftClick.eventID = EventTriggerType.PointerClick;
            leftClick.callback.AddListener(new UnityAction<BaseEventData>((x) => { OnMouseClickPage(false); }));
            leftHotSpot.GetComponent<EventTrigger>().triggers.Add(leftClick);

            EventTrigger.Entry rightClick = new EventTrigger.Entry();
            rightClick.eventID = EventTriggerType.PointerClick;
            rightClick.callback.AddListener(new UnityAction<BaseEventData>((x) => { OnMouseClickPage(true); }));
            rightHotSpot.GetComponent<EventTrigger>().triggers.Add(rightClick);*/
        }

        void Update()
        {
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }            

            if (_pageFlipping)
            {
                if (_isAutoFlipping)
                {
                    // 自动翻页
                    if (_elapseAutoFlipTime < _autoFlipTime)
                    {
                        _elapseAutoFlipTime += Time.deltaTime;
                        float progress = Mathf.Min(1, _elapseAutoFlipTime / _autoFlipTime);
                        Vector3 tarPos = Vector3.Lerp(_autoFlipSrcPos, _autoFlipDstPos, progress);                        
                        UpdateFlipping(tarPos);
                    }
                    else if (_autoFlipReleased == false)
                    {
                        _autoFlipReleased = true;
                        ReleasePage();                        
                    }                    
                }
                else if (interactable)
                {
                    // 手动翻页
                    UpdateFlipping(transformPointMousePosition(Input.mousePosition));
                }                
            }
        }

        void UpdateFlipping(Vector3 tarPos)
        {
            _f = Vector3.Lerp(_f, tarPos, Time.deltaTime * 10);

            if (_mode == FlipMode.RightToLeft)
                UpdateFlipRight2LeftToPoint(_f);
            else
                UpdateFlipLeft2RightToPoint(_f);
        }

        public void UpdateFlipLeft2RightToPoint(Vector3 followLocation)
        {
            _mode = FlipMode.LeftToRight;
            _f = followLocation;
            shadowLTR.transform.SetParent(clippingPlane.transform, true);
            shadowLTR.transform.localPosition = new Vector3(0, 0, 0);
            shadowLTR.transform.localEulerAngles = new Vector3(0, 0, 0);
            leftOnFlip.transform.SetParent(clippingPlane.transform, true);

            rightOnFlip.transform.SetParent(bookPanel.transform, true);
            left.transform.SetParent(bookPanel.transform, true);

            _c = Calc_C_Position(followLocation);
            Vector3 t1;
            float T0_T1_Angle = Calc_T0_T1_Angle(_c, _ebl, out t1);
            if (T0_T1_Angle < 0) T0_T1_Angle += 180;

            clippingPlane.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle - 90);
            clippingPlane.transform.position = bookPanel.TransformPoint(t1);

            //page position and angle
            leftOnFlip.transform.position = bookPanel.TransformPoint(_c);
            float C_T1_dy = t1.y - _c.y;
            float C_T1_dx = t1.x - _c.x;
            float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
            leftOnFlip.transform.eulerAngles = new Vector3(0, 0, C_T1_Angle - 180);

            nextPageClip.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle - 90);
            nextPageClip.transform.position = bookPanel.TransformPoint(t1);
            left.transform.SetParent(nextPageClip.transform, true);
            rightOnFlip.transform.SetParent(clippingPlane.transform, true);
            rightOnFlip.transform.SetAsFirstSibling();

            shadowLTR.rectTransform.SetParent(leftOnFlip.rectTransform, true);
        }

        public void UpdateFlipRight2LeftToPoint(Vector3 followLocation)
        {
            _mode = FlipMode.RightToLeft;
            _f = followLocation;
            shadow.transform.SetParent(clippingPlane.transform, true);
            shadow.transform.localPosition = new Vector3(0, 0, 0);
            shadow.transform.localEulerAngles = new Vector3(0, 0, 0);
            rightOnFlip.transform.SetParent(clippingPlane.transform, true);

            leftOnFlip.transform.SetParent(bookPanel.transform, true);
            right.transform.SetParent(bookPanel.transform, true);
            _c = Calc_C_Position(followLocation);
            Vector3 t1;
            float T0_T1_Angle = Calc_T0_T1_Angle(_c, _ebr, out t1);
            if (T0_T1_Angle >= -90) T0_T1_Angle -= 180;

            clippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
            clippingPlane.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
            clippingPlane.transform.position = bookPanel.TransformPoint(t1);

            //page position and angle
            rightOnFlip.transform.position = bookPanel.TransformPoint(_c);
            float C_T1_dy = t1.y - _c.y;
            float C_T1_dx = t1.x - _c.x;
            float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
            rightOnFlip.transform.eulerAngles = new Vector3(0, 0, C_T1_Angle);

            nextPageClip.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
            nextPageClip.transform.position = bookPanel.TransformPoint(t1);
            right.transform.SetParent(nextPageClip.transform, true);
            leftOnFlip.transform.SetParent(clippingPlane.transform, true);
            leftOnFlip.transform.SetAsFirstSibling();

            shadow.rectTransform.SetParent(rightOnFlip.rectTransform, true);
        }

        private float Calc_T0_T1_Angle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
        {
            Vector3 t0 = (c + bookCorner) / 2;
            float T0_CORNER_dy = bookCorner.y - t0.y;
            float T0_CORNER_dx = bookCorner.x - t0.x;
            float T0_CORNER_Angle = Mathf.Atan2(T0_CORNER_dy, T0_CORNER_dx);
            float T0_T1_Angle = 90 - T0_CORNER_Angle;

            float T1_X = t0.x - T0_CORNER_dy * Mathf.Tan(T0_CORNER_Angle);
            T1_X = normalizeT1X(T1_X, bookCorner, _sb);
            t1 = new Vector3(T1_X, _sb.y, 0);
            ////////////////////////////////////////////////
            //clipping plane angle=T0_T1_Angle
            float T0_T1_dy = t1.y - t0.y;
            float T0_T1_dx = t1.x - t0.x;
            T0_T1_Angle = Mathf.Atan2(T0_T1_dy, T0_T1_dx) * Mathf.Rad2Deg;
            return T0_T1_Angle;
        }

        private float normalizeT1X(float t1, Vector3 corner, Vector3 sb)
        {
            if (t1 > sb.x && sb.x > corner.x)
                return sb.x;
            if (t1 < sb.x && sb.x < corner.x)
                return sb.x;
            return t1;
        }

        private Vector3 Calc_C_Position(Vector3 followLocation)
        {
            Vector3 c;
            _f = followLocation;
            float F_SB_dy = _f.y - _sb.y;
            float F_SB_dx = _f.x - _sb.x;
            float F_SB_Angle = Mathf.Atan2(F_SB_dy, F_SB_dx);
            Vector3 r1 = new Vector3(_radius1 * Mathf.Cos(F_SB_Angle), _radius1 * Mathf.Sin(F_SB_Angle), 0) + _sb;

            float F_SB_distance = Vector2.Distance(_f, _sb);
            if (F_SB_distance < _radius1)
                c = _f;
            else
                c = r1;
            float F_ST_dy = c.y - _st.y;
            float F_ST_dx = c.x - _st.x;
            float F_ST_Angle = Mathf.Atan2(F_ST_dy, F_ST_dx);
            Vector3 r2 = new Vector3(_radius2 * Mathf.Cos(F_ST_Angle),
               _radius2 * Mathf.Sin(F_ST_Angle), 0) + _st;
            float C_ST_distance = Vector2.Distance(c, _st);
            if (C_ST_distance > _radius2)
                c = r2;
            return c;
        }

        // 是否已经是最后一页
        public bool IsLastPage()
        {
            return _curPage == (papers.Count / 2 - 1);
        }

        public void DragRightPageToPoint(Vector3 point)
        {
            if (IsLastPage())
                return;

            if (_pageFlipping)
            {
                return;
            }

            _pageFlipping = true;
            _mode = FlipMode.RightToLeft;
            _f = point;

            nextPageClip.rectTransform.pivot = new Vector2(0, 0.12f);
            clippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);

            leftOnFlip.gameObject.SetActive(true);
            leftOnFlip.rectTransform.pivot = new Vector2(0, 0);
            leftOnFlip.transform.position = right.transform.position;
            leftOnFlip.transform.eulerAngles = new Vector3(0, 0, 0);
            leftOnFlip.transform.SetAsFirstSibling();

            rightOnFlip.gameObject.SetActive(true);
            rightOnFlip.transform.position = right.transform.position;
            rightOnFlip.transform.eulerAngles = new Vector3(0, 0, 0);

           // Left.rectTransform.pivot = new Vector2(0, 0);

            left.transform.SetAsFirstSibling();
            UpdatePagesOnFlip();

            if (enableShadowEffect)
                shadow.gameObject.SetActive(true);

            UpdateFlipRight2LeftToPoint(_f);
        }

        public bool IsFirstPage()
        {
            return _curPage == 0;
        }

        public void DragLeftPageToPoint(Vector3 point)
        {
            if (IsFirstPage())
                return;

            if (_pageFlipping)
            {
                return;
            }

            _pageFlipping = true;
            _mode = FlipMode.LeftToRight;
            _f = point;

            nextPageClip.rectTransform.pivot = new Vector2(1, 0.12f);
            clippingPlane.rectTransform.pivot = new Vector2(0, 0.35f);

            rightOnFlip.gameObject.SetActive(true);
            rightOnFlip.transform.position = left.transform.position;
            rightOnFlip.transform.eulerAngles = new Vector3(0, 0, 0);
            rightOnFlip.transform.SetAsFirstSibling();

            leftOnFlip.gameObject.SetActive(true);
            leftOnFlip.rectTransform.pivot = new Vector2(1, 0);
            leftOnFlip.transform.position = left.transform.position;
            leftOnFlip.transform.eulerAngles = new Vector3(0, 0, 0);

            //Left.rectTransform.pivot = new Vector2(1, 0);

            right.transform.SetAsFirstSibling();
            UpdatePagesOnFlip();

            if (enableShadowEffect)
                shadowLTR.gameObject.SetActive(true);

            UpdateFlipLeft2RightToPoint(_f);
        }

        public void UpdatePage()
        {
            left.sprite = papers[_curPage * 2];
            right.sprite = papers[_curPage * 2 + 1];

            if (onPageShow != null)
            {
                onPageShow.Invoke(_curPage, left, right);
            }            
        }

        void UpdatePagesOnFlip()
        {
            if (_mode == FlipMode.LeftToRight)
            {
                left.sprite = papers[CurPaper - 3];
                leftOnFlip.sprite = papers[CurPaper - 2];
                rightOnFlip.sprite = papers[CurPaper - 1];
                right.sprite = papers[CurPaper];         
            }
            else
            {
                left.sprite = papers[CurPaper - 1];
                leftOnFlip.sprite = papers[CurPaper];
                rightOnFlip.sprite = papers[CurPaper + 1];
                right.sprite = papers[CurPaper + 2];
            }
        }

        public void OnMouseDragLeftPage()
        {
            if (interactable)
                DragLeftPageToPoint(transformPointMousePosition(Input.mousePosition));
        }

        public void OnMouseDragRightPage()
        {
            if (interactable)
                DragRightPageToPoint(transformPointMousePosition(Input.mousePosition));
        }

        public void OnMouseRelease()
        {
            if (interactable)
                ReleasePage();
        }

        public void ReleasePage()
        {
            if (_pageFlipping)
            {                
                float distanceToLeft = Vector2.Distance(_c, _ebl);
                float distanceToRight = Vector2.Distance(_c, _ebr);
                if (distanceToRight < distanceToLeft && _mode == FlipMode.RightToLeft)
                    TweenBack();
                else if (distanceToRight > distanceToLeft && _mode == FlipMode.LeftToRight)
                    TweenBack();
                else
                    TweenForward();
            }
        }        

        public void TweenForward()
        {
            if (_mode == FlipMode.RightToLeft)
                _currentCoroutine = StartCoroutine(TweenTo(_ebl, 0.15f, () => { OnPageFlipped(); }));
            else
                _currentCoroutine = StartCoroutine(TweenTo(_ebr, 0.15f, () => { OnPageFlipped(); }));
        }

        public void TweenBack()
        {
            if (_mode == FlipMode.RightToLeft)
            {
                _currentCoroutine = StartCoroutine(TweenTo(_ebr, 0.15f, () => { OnPageRestored(); }));
            }
            else
            {
                _currentCoroutine = StartCoroutine(TweenTo(_ebl, 0.15f, () => { OnPageRestored(); }));
            }
        }

        void OnPageFlipped()
        {
            _pageFlipping = false;
            _isAutoFlipping = false;

            if (_mode == FlipMode.RightToLeft)
            {
                _curPage += 1;                
            }
            else
            {
                _curPage -= 1;
            }

            left.transform.SetParent(bookPanel.transform, true);
            leftOnFlip.transform.SetParent(bookPanel.transform, true);
            left.transform.SetParent(bookPanel.transform, true);
            leftOnFlip.gameObject.SetActive(false);
            rightOnFlip.gameObject.SetActive(false);
            rightOnFlip.transform.SetParent(bookPanel.transform, true);
            right.transform.SetParent(bookPanel.transform, true);

            UpdatePage();

            shadow.gameObject.SetActive(false);
            shadowLTR.gameObject.SetActive(false);

            if (onPageFlipped != null)
                onPageFlipped.Invoke();
        }

        void OnPageRestored()
        {
            UpdatePage();

            if (_mode == FlipMode.LeftToRight)
            {
                left.transform.SetParent(bookPanel.transform);
                leftOnFlip.transform.SetParent(bookPanel.transform);
            }
            else
            {
                right.transform.SetParent(bookPanel.transform);
                rightOnFlip.transform.SetParent(bookPanel.transform);
            }            

            leftOnFlip.gameObject.SetActive(false);
            rightOnFlip.gameObject.SetActive(false);
            _pageFlipping = false;
            _isAutoFlipping = false;
        }

        public IEnumerator TweenTo(Vector3 to, float duration, System.Action onFinish)
        {
            int steps = (int)(duration / 0.025f);
            Vector3 displacement = (to - _f) / steps;
            for (int i = 0; i < steps - 1; i++)
            {
                if (_mode == FlipMode.RightToLeft)
                    UpdateFlipRight2LeftToPoint(_f + displacement);
                else
                    UpdateFlipLeft2RightToPoint(_f + displacement);

                yield return new WaitForSeconds(0.025f);
            }
            if (onFinish != null)
                onFinish();
        }

        public Vector3 transformPointMousePosition(Vector3 mouseScreenPos)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Vector3 mouseWorldPos = canvas.worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, canvas.planeDistance));
                Vector2 localPos = bookPanel.InverseTransformPoint(mouseWorldPos);

                return localPos;
            }
            else if (canvas.renderMode == RenderMode.WorldSpace)
            {
                Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 globalEBR = transform.TransformPoint(_ebr);
                Vector3 globalEBL = transform.TransformPoint(_ebl);
                Vector3 globalSt = transform.TransformPoint(_st);
                Plane p = new Plane(globalEBR, globalEBL, globalSt);
                float distance;
                p.Raycast(ray, out distance);
                Vector2 localPos = bookPanel.InverseTransformPoint(ray.GetPoint(distance));
                return localPos;
            }
            else
            {
                //Screen Space Overlay
                Vector2 localPos = bookPanel.InverseTransformPoint(mouseScreenPos);
                return localPos;
            }
        }

        /*public void OnMouseClickPage(bool clickRight 是否点击右页)
        {
            if (interactable == false || _pageFlipping || onPageClicked == null)
            {
                return;
            }

            if (_curPage % 2 == 0)
            {
                if (clickRight)
                {
                    //Debug.Log("this is page: " + curPage);
                    onPageClicked.Invoke(_curPage);
                }
                else
                {
                    if (_curPage > 0)
                    {
                        //Debug.Log("this is page: " + (curPage - 1));
                        onPageClicked.Invoke(_curPage - 1);
                    }                    
                }
            }
            else
            {
                if (clickRight == false)
                {
                    //Debug.Log("this is page: " + curPage);
                    onPageClicked.Invoke(_curPage);
                }
                else
                {
                    if (_curPage < papers.Count - 1)
                    {
                        //Debug.Log("this is page: " + (curPage + 1));
                        onPageClicked.Invoke(_curPage + 1);
                    }
                }
            }
        }*/

        /// <summary>
        /// 自动向左翻页
        /// </summary>
        public void AutoFlipRight(float flipTime)
        {
            if (_pageFlipping)
            {
                return;
            }

            if (IsLastPage())
            {
                return;
            }

            if (flipTime <= 0)
            {
                return;
            }

            _autoFlipReleased = false;
            _isAutoFlipping = true;
            _elapseAutoFlipTime = 0;
            _autoFlipTime = flipTime;            
            _autoFlipSrcPos = new Vector3(bookPanel.anchoredPosition.x, bookPanel.anchoredPosition.y, 0) + _ebr;
            _autoFlipDstPos = new Vector3(bookPanel.anchoredPosition.x, bookPanel.anchoredPosition.y, 0) + _ebl;
            DragRightPageToPoint(_autoFlipSrcPos);
        }
/*
        IEnumerator AutoFlipRightImpl(Vector3 from, Vector3 to, float flipTime)
        {
            _isAuto = true;
            float elapseTime = 0;
            DragRightPageToPoint(from);
            
            do
            {
                yield return new WaitForEndOfFrame();
                elapseTime += Time.deltaTime;
                float progress = Mathf.Min(1, elapseTime / flipTime);
                Vector3 tarPos = Vector3.Lerp(from, to, progress);
                Debug.Log(tarPos);
                UpdateFlipping(tarPos);
            } while (elapseTime < flipTime);

            ReleasePage();
            _isAuto = false;
        }*/

        /// <summary>
        /// 自动向右翻页
        /// </summary>
        public void AutoFlipLeft(float flipTime)
        {
            if (_pageFlipping)
            {
                return;
            }

            if (IsFirstPage())
            {
                return;
            }

            if (flipTime <= 0)
            {
                return;
            }

            _autoFlipReleased = false;
            _isAutoFlipping = true;
            _elapseAutoFlipTime = 0;
            _autoFlipTime = flipTime;
            _autoFlipSrcPos = new Vector3(bookPanel.anchoredPosition.x, bookPanel.anchoredPosition.y, 0) + _ebl;
            _autoFlipDstPos = new Vector3(bookPanel.anchoredPosition.x, bookPanel.anchoredPosition.y, 0) + _ebr;
            DragLeftPageToPoint(_autoFlipSrcPos);
        }
        /*
        IEnumerator AutoFlipLeftImpl(Vector3 from, Vector3 to, float flipTime)
        {
            _isAuto = true;
            float elapseTime = 0;
            DragLeftPageToPoint(from);

            do
            {
                yield return new WaitForEndOfFrame();
                elapseTime += Time.deltaTime;
                float progress = Mathf.Min(1, elapseTime / flipTime);
                Vector3 tarPos = Vector3.Lerp(from, to, progress);
                Debug.Log(tarPos);
                UpdateFlipping(tarPos);
            } while (elapseTime < flipTime);

            ReleasePage();
            _isAuto = false;
        }*/
    }
}