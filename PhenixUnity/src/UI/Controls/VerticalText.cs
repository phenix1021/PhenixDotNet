using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 支持文字纵向排版
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Phenix/UI/VerticalText")]
    public class VerticalText : BaseMeshEffect
    {
        public enum VerticalTextDirection
        {
            RIGHT_LEFT = 0,
            LEFT_RIGHT,
        }

        public VerticalTextDirection textDirection = VerticalTextDirection.RIGHT_LEFT;

        public float colSpacingModifier = 1;
        public float charSpacingModifier = 1;

        float _colSpacing = 1;   // 列间距
        float _charSpacing = 1;  // 字间距
        float _xOffsetFromPivot = 0;
        float _yOffsetFromPivot = 0;

        Text _text;
        TextGenerator _textGenerator;
        List<UILineInfo> _lines = new List<UILineInfo>();

        protected override void Awake()
        {
            _text = GetComponent<Text>();
        }

        public override void ModifyMesh(VertexHelper helper)
        {
            if (!IsActive())
                return;

            _text.rectTransform.pivot.Set(0.5f, 0.5f);

            _colSpacing = _text.fontSize * _text.lineSpacing * colSpacingModifier;
            _charSpacing = _text.fontSize * charSpacingModifier;

            if (textDirection == VerticalTextDirection.RIGHT_LEFT)
            {
                // 从右往左
                _xOffsetFromPivot = _text.rectTransform.rect.width / 2 - _text.fontSize / 2;
                _yOffsetFromPivot = _text.rectTransform.rect.height / 2 - _text.fontSize / 2;
            }
            else
            {
                // 从左往右
                _xOffsetFromPivot = -_text.rectTransform.rect.width / 2 + _text.fontSize / 2;
                _yOffsetFromPivot = _text.rectTransform.rect.height / 2 - _text.fontSize / 2;
            }

            _textGenerator = _text.cachedTextGenerator;
            _lines.Clear();
            _textGenerator.GetLines(_lines);

            for (int i = 0; i < _lines.Count; i++)
            {
                // 遍历每一行文字
                UILineInfo line = _lines[i];

                int rowIdx = i;
                int rowCharIdx = 0;
                int totalCharIdx = line.startCharIdx;

                if (i + 1 < _lines.Count)
                {
                    // 如果不是最后一行
                    UILineInfo nextLine = _lines[i + 1];

                    for (; totalCharIdx < nextLine.startCharIdx - 1; totalCharIdx++)
                    {
                        modifyText(helper, totalCharIdx, rowCharIdx++, rowIdx);
                    }
                }
                else if (i + 1 == _lines.Count)
                {
                    // 最后一行                
                    for (; totalCharIdx < _textGenerator.characterCountVisible; totalCharIdx++)
                    {
                        modifyText(helper, totalCharIdx, rowCharIdx++, rowIdx);
                    }
                }
            }
        }

        void modifyText(VertexHelper helper, int totalCharIdx, int rowCharIdx, int rowIdx)
        {
            UIVertex lb = new UIVertex();
            helper.PopulateUIVertex(ref lb, totalCharIdx * 4);

            UIVertex lt = new UIVertex();
            helper.PopulateUIVertex(ref lt, totalCharIdx * 4 + 1);

            UIVertex rt = new UIVertex();
            helper.PopulateUIVertex(ref rt, totalCharIdx * 4 + 2);

            UIVertex rb = new UIVertex();
            helper.PopulateUIVertex(ref rb, totalCharIdx * 4 + 3);

            Vector3 center = Vector3.Lerp(lb.position, rt.position, 0.5f);
            Matrix4x4 move = Matrix4x4.TRS(-center, Quaternion.identity, Vector3.one);

            float x, y;
            CalcXYInRect(rowIdx, rowCharIdx, out x, out y);
            Vector3 pos = new Vector3(x, y, 0);
            Matrix4x4 place = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
            Matrix4x4 transform = place * move;

            lb.position = transform.MultiplyPoint(lb.position);
            lt.position = transform.MultiplyPoint(lt.position);
            rt.position = transform.MultiplyPoint(rt.position);
            rb.position = transform.MultiplyPoint(rb.position);

            helper.SetUIVertex(lb, totalCharIdx * 4);
            helper.SetUIVertex(lt, totalCharIdx * 4 + 1);
            helper.SetUIVertex(rt, totalCharIdx * 4 + 2);
            helper.SetUIVertex(rb, totalCharIdx * 4 + 3);
        }


        void CalcXYInRect(int rowIdx, int rowCharIdx, out float x, out float y)
        {
            if (textDirection == VerticalTextDirection.RIGHT_LEFT)
            {
                x = -rowIdx * _colSpacing + _xOffsetFromPivot;
                y = -rowCharIdx * _charSpacing + _yOffsetFromPivot;
            }
            else
            {
                x = rowIdx * _colSpacing + _xOffsetFromPivot;
                y = -rowCharIdx * _charSpacing + _yOffsetFromPivot;
            }
        }
    }
}