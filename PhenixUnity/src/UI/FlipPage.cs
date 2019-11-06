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
    public class UnityEventPageShow : UnityEvent<int/*页面号*/, Image> { }

    [AddComponentMenu("Phenix/UI/FlipPage")]
    public class FlipPage : MonoBehaviour
    {
        public Canvas canvas;
                
        public RectTransform bookPanel;

        public Sprite placeHold;
        public List<Sprite> bookPages = new List<Sprite>();

        public bool interactable = true;
        public bool enableShadowEffect = true;
                
        public int curPage = 0;

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

        float _radius1, _radius2;

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

        bool _pageFlipping = false;

        //current flip mode
        FlipMode _mode;

        Coroutine _currentCoroutine;

        public UnityEvent onPageFlipped;                          // 翻页完成
        public UnityEventPageClicked onPageClicked;               // 点击页面
        public UnityEventPageShow onPageShow;                     // 页面显示

        public int TotalPageCount { get { return bookPages.Count; } }
        public Vector3 EndBottomLeft { get { return _ebl; } }
        public Vector3 EndBottomRight { get { return _ebr; } }
        public float Height { get { return bookPanel.rect.height; } }

        void Start()
        {
            float pageWidth = (bookPanel.rect.width - 1) / 2;
            float pageHeight = bookPanel.rect.height;

            leftOnFlip.gameObject.SetActive(false);
            rightOnFlip.gameObject.SetActive(false);

            if (bookPages.Count <= curPage)
            {
                curPage = bookPages.Count > 0 ? bookPages.Count - 1 : 0;
            }

            UpdateSprites();

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

            EventTrigger.Entry leftClick = new EventTrigger.Entry();
            leftClick.eventID = EventTriggerType.PointerClick;
            leftClick.callback.AddListener(new UnityAction<BaseEventData>((x) => { OnMouseClickPage(false); }));
            leftHotSpot.GetComponent<EventTrigger>().triggers.Add(leftClick);

            EventTrigger.Entry rightClick = new EventTrigger.Entry();
            rightClick.eventID = EventTriggerType.PointerClick;
            rightClick.callback.AddListener(new UnityAction<BaseEventData>((x) => { OnMouseClickPage(true); }));
            rightHotSpot.GetComponent<EventTrigger>().triggers.Add(rightClick);
        }

        void Update()
        {
            if (_pageFlipping && interactable)
            {
                UpdateBook();
            }
        }

        void UpdateBook()
        {
            _f = Vector3.Lerp(_f, transformPointMousePosition(Input.mousePosition), Time.deltaTime * 10);

            if (_mode == FlipMode.RightToLeft)
                UpdateBookRTLToPoint(_f);
            else
                UpdateBookLTRToPoint(_f);
        }

        public void UpdateBookLTRToPoint(Vector3 followLocation)
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

        public void UpdateBookRTLToPoint(Vector3 followLocation)
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

        public void DragRightPageToPoint(Vector3 point)
        {
            if (curPage >= bookPages.Count - 1)
                return;

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
            UpdateSpritesOnFlip();

            if (enableShadowEffect)
                shadow.gameObject.SetActive(true);

            UpdateBookRTLToPoint(_f);
        }

        public void DragLeftPageToPoint(Vector3 point)
        {
            if (curPage <= 0)
                return;
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
            UpdateSpritesOnFlip();

            if (enableShadowEffect)
                shadowLTR.gameObject.SetActive(true);

            UpdateBookLTRToPoint(_f);
        }

        void UpdateSprites()
        {
            if (bookPages.Count == 0)
            {
                left.enabled = right.enabled = false;
            }
            else if (curPage <= 0)
            {
                left.sprite = placeHold;
                if (placeHold == null)
                {
                    left.enabled = false;
                }

                right.enabled = true;
                right.sprite = bookPages[0];

                if (onPageShow != null)
                {
                    onPageShow.Invoke(0, right);
                }
            }
            else if (curPage >= bookPages.Count - 1)
            {
                left.enabled = true;
                left.sprite = bookPages[bookPages.Count - 1];

                if (onPageShow != null)
                {
                    onPageShow.Invoke(bookPages.Count - 1, left);
                }

                right.sprite = placeHold;
                if (placeHold == null)
                {
                    right.enabled = false;
                }
            }
            else
            {
                left.enabled = right.enabled = true;
                if (curPage % 2 > 0)
                {
                    left.sprite = bookPages[curPage];
                    right.sprite = bookPages[curPage + 1];

                    if (onPageShow != null)
                    {
                        onPageShow.Invoke(curPage, left);
                        onPageShow.Invoke(curPage + 1, right);
                    }
                }
                else
                {
                    left.sprite = bookPages[curPage - 1];
                    right.sprite = bookPages[curPage];

                    if (onPageShow != null)
                    {
                        onPageShow.Invoke(curPage - 1, left);
                        onPageShow.Invoke(curPage, right);
                    }
                }
            }
        }

        void UpdateSpritesOnFlip()
        {
            if (_mode == FlipMode.LeftToRight)
            {
                if (curPage % 2 == 0)
                {
                    rightOnFlip.sprite = bookPages[curPage - 1];

                    if (curPage >= 2)
                    {
                        leftOnFlip.enabled = true;
                        leftOnFlip.sprite = bookPages[curPage - 2];
                    }
                    else
                    {
                        leftOnFlip.sprite = placeHold;
                        if (placeHold == null)
                        {
                            leftOnFlip.enabled = false;
                        }
                    }

                    if (curPage >= 3)
                    {
                        left.enabled = true;
                        left.sprite = bookPages[curPage - 3];
                    }
                    else
                    {
                        left.sprite = placeHold;
                        if (placeHold == null)
                        {
                            left.enabled = false;
                        }
                    }
                }
                else
                {
                    rightOnFlip.sprite = bookPages[curPage];
                    if (curPage >= 1)
                    {
                        leftOnFlip.sprite = bookPages[curPage - 1];
                        leftOnFlip.enabled = true;
                    }
                    else
                    {
                        leftOnFlip.sprite = placeHold;
                        if (placeHold == null)
                        {
                            leftOnFlip.enabled = false;
                        }
                    }

                    if (curPage >= 2)
                    {
                        left.enabled = true;
                        left.sprite = bookPages[curPage - 2];
                    }
                    else
                    {
                        left.sprite = placeHold;
                        if (placeHold == null)
                        {
                            left.enabled = false;
                        }
                    }
                }
            }
            else
            {
                if (curPage % 2 == 0)
                {
                    leftOnFlip.sprite = bookPages[curPage];

                    if (curPage < bookPages.Count - 1)
                    {
                        rightOnFlip.sprite = bookPages[curPage + 1];
                        rightOnFlip.enabled = true;
                    }
                    else
                    {
                        rightOnFlip.sprite = placeHold;
                        if (placeHold == null)
                        {
                            rightOnFlip.enabled = false;
                        }
                    }

                    if (curPage < bookPages.Count - 2)
                    {
                        right.sprite = bookPages[curPage + 2];
                        right.enabled = true;
                    }
                    else
                    {
                        right.sprite = placeHold;
                        if (placeHold == null)
                        {
                            right.enabled = false;
                        }
                    }
                }
                else
                {
                    leftOnFlip.sprite = bookPages[curPage + 1];

                    if (curPage < bookPages.Count - 2)
                    {
                        rightOnFlip.sprite = bookPages[curPage + 2];
                        rightOnFlip.enabled = true;
                    }
                    else
                    {
                        rightOnFlip.sprite = placeHold;
                        if (placeHold == null)
                        {
                            rightOnFlip.enabled = false;
                        }
                    }

                    if (curPage < bookPages.Count - 3)
                    {
                        right.sprite = bookPages[curPage + 3];
                        right.enabled = true;
                    }
                    else
                    {
                        right.sprite = placeHold;
                        if (placeHold == null)
                        {
                            right.enabled = false;
                        }
                    }
                }
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

            if (_mode == FlipMode.RightToLeft)
            {
                curPage += 2;
                if (curPage > bookPages.Count - 1)
                {
                    curPage = bookPages.Count - 1;
                }
            }
            else
            {
                curPage -= 2;
                if (curPage < 0)
                {
                    curPage = 0;
                }
            }

            left.transform.SetParent(bookPanel.transform, true);
            leftOnFlip.transform.SetParent(bookPanel.transform, true);
            left.transform.SetParent(bookPanel.transform, true);
            leftOnFlip.gameObject.SetActive(false);
            rightOnFlip.gameObject.SetActive(false);
            rightOnFlip.transform.SetParent(bookPanel.transform, true);
            right.transform.SetParent(bookPanel.transform, true);

            UpdateSprites();

            shadow.gameObject.SetActive(false);
            shadowLTR.gameObject.SetActive(false);

            if (onPageFlipped != null)
                onPageFlipped.Invoke();
        }

        void OnPageRestored()
        {
            UpdateSprites();

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
        }

        public IEnumerator TweenTo(Vector3 to, float duration, System.Action onFinish)
        {
            int steps = (int)(duration / 0.025f);
            Vector3 displacement = (to - _f) / steps;
            for (int i = 0; i < steps - 1; i++)
            {
                if (_mode == FlipMode.RightToLeft)
                    UpdateBookRTLToPoint(_f + displacement);
                else
                    UpdateBookLTRToPoint(_f + displacement);

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

        public void OnMouseClickPage(bool clickRight/*是否点击右页*/)
        {
            if (interactable == false || _pageFlipping || onPageClicked == null)
            {
                return;
            }

            if (curPage % 2 == 0)
            {
                if (clickRight)
                {
                    //Debug.Log("this is page: " + curPage);
                    onPageClicked.Invoke(curPage);
                }
                else
                {
                    if (curPage > 0)
                    {
                        //Debug.Log("this is page: " + (curPage - 1));
                        onPageClicked.Invoke(curPage - 1);
                    }                    
                }
            }
            else
            {
                if (clickRight == false)
                {
                    //Debug.Log("this is page: " + curPage);
                    onPageClicked.Invoke(curPage);
                }
                else
                {
                    if (curPage < bookPages.Count - 1)
                    {
                        //Debug.Log("this is page: " + (curPage + 1));
                        onPageClicked.Invoke(curPage + 1);
                    }
                }
            }
        }
    }
}