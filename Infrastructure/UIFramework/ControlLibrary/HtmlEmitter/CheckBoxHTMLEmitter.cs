﻿using System;
using System.Text;
using System.Web.Mvc;

namespace Controls.ControlLibrary
{
    internal class CheckBoxHTMLEmitter : ControlHTMLEmitter
    {
        #region "Member Variables"

        private CheckBoxPropertyBag _propertyBag;
        private string _controlID;

        #endregion "Member Variables"

        #region "Constructors"

        public CheckBoxHTMLEmitter(string value, CheckBoxPropertyBag propertyBag)
            : base(propertyBag.Validators, propertyBag.Mandatory)
        {
            this._propertyBag = propertyBag;
            this.Value = value;
        }

        #endregion "Constructors"

        #region "Implemented Methods"

        public override void Emit(out string controlHTMLString)
        {
            controlHTMLString = string.Empty;
            if (_propertyBag.Visibility)
            {
                TagBuilder divTag = new TagBuilder(TAG_DIV);
                this.SetControlCssClasses(divTag, this._propertyBag.CssClass, this._propertyBag.ControlErrorCssClass);
                StringBuilder control = new StringBuilder();
                control.Append(BuildLabel());
                control.Append(BuildScript());
                divTag.InnerHtml = control.ToString();
                SetInnerHtml(divTag, GetErrorLabel(this._propertyBag), true);
                controlHTMLString = divTag.ToString();
            }
        }

        #endregion "Implemented Methods"

        #region "Private Methods"

        private string BuildLabel()
        {
            TagBuilder label = new TagBuilder(TAG_LABEL);
            StringBuilder control = new StringBuilder();
            control.Append(BuildCheckBox());
            control.Append(BuildSpan());
            control.Append(BuildHiddenElement());
            label.InnerHtml = control.ToString();
            return label.ToString();
        }

        private string BuildSpan()
        {
            TagBuilder spanTag = new TagBuilder(TAG_SPAN);
            spanTag.InnerHtml = this._propertyBag.Label;
            return spanTag.ToString();
        }

        private string BuildHiddenElement()
        {
            TagBuilder hidden;
            this.BuildHiddenTag("hid_" + this._propertyBag.ControlName, this._propertyBag.ControlName, this.Value.ToLower(), out hidden);
            return hidden.ToString(TagRenderMode.SelfClosing);
        }

        private string BuildCheckBox()
        {
            TagBuilder controlTag = new TagBuilder(TAG_INPUT);
            this.SetAttribute(controlTag, ATTRIBUTE_TYPE, ATTR_VAL_CHECKBOX);
            this.SetID(controlTag, this._propertyBag.ControlName, out _controlID);
            this.SetAttribute(controlTag, ATTRIBUTE_NAME, "hid_" + this._propertyBag.ControlName);

            if (this._propertyBag.Style != null)
            {
                this.SetAttribute(controlTag, ATTRIBUTE_STYLE, this._propertyBag.Style.GetStyle());
            }

            //this.SetValidations(controlTag);

            this.SetAttribute(controlTag, ATTRIBUTE_TABINDEX, this._propertyBag.TabIndex);

            this.SetAttribute(controlTag, ATTRIBUTE_TITLE, this._propertyBag.ToolTip);

            this.SetAttribute(controlTag, ATTRIBUTE_READONLY, ATTR_VAL_READONLY, this._propertyBag.ReadOnly);
            this.SetAttribute(controlTag, ATTRIBUTE_DISABLED, ATTR_VAL_DISABLED, !this._propertyBag.Enabled);

            this.SetAttribute(controlTag, ATTRIBUTE_CHECKED, ATTR_VAL_CHECKED, Convert.ToBoolean(this.Value));

            //this is required for mvc default model binding
            this.SetAttribute(controlTag, ATTRIBUTE_VALUE, VAL_TRUE);

            this.SetCustomAttributes(controlTag, _propertyBag.Attributes);
            this.SetChangeEvent(controlTag);
            this.SetFunction(controlTag, ATTRIBUTE_ONCLICK, _propertyBag.OnClickFunction);
            return controlTag.ToString(TagRenderMode.SelfClosing);
        }

        private string BuildScript()
        {
            TagBuilder ScriptTag = new TagBuilder(TAG_SCRIPT);
            this.SetAttribute(ScriptTag, ATTRIBUTE_LANG, SCRIPT_NAME);
            StringBuilder script = new StringBuilder();
            ScriptTag.InnerHtml = script.ToString();
            return Convert.ToString(ScriptTag);
        }

        #endregion "Private Methods"
    }
}