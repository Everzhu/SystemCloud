//修正jquery.validate.js在IE7或IE8兼容模式下报错的BUG，注意，必须删除jquery.validate.min.js否则一样报错
if (!window.Element || !window.Element.prototype || !window.Element.prototype.hasAttribute)
{

    (function ()
    {
        function hasAttribute(attrName)
        {
            return typeof this[attrName] !== 'undefined'; // You may also be able to check getAttribute() against null, though it is possible this could cause problems for any older browsers (if any) which followed the old DOM3 way of returning the empty string for an empty string (yet did not possess hasAttribute as per our checks above). See https://developer.mozilla.org/en-US/docs/Web/API/Element.getAttribute
        }
        var inputs = document.getElementsByTagName('input');
        for (var i = 0; i < inputs.length; i++)
        {
            inputs[i].hasAttribute = hasAttribute;
        }
    }());

}