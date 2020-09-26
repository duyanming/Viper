/**
* jQuery ligerUI 1.3.2
* 
* http://ligerui.com
*  
* Author daomi 2015 [ gd_star@163.com ] 
* 
*/
(function ($)
{

    $.fn.ligerButton = function (options)
    {
        return $.ligerui.run.call(this, "ligerButton", arguments);
    };
    $.fn.ligerGetButtonManager = function ()
    {
        return $.ligerui.run.call(this, "ligerGetButtonManager", arguments);
    };

    $.ligerDefaults.Button = {
        width: 60,
        text: 'Button',
        disabled: false,
        click: null,
        icon : null
    };

    $.ligerMethos.Button = {};

    $.ligerui.controls.Button = function (element, options)
    {
        $.ligerui.controls.Button.base.constructor.call(this, element, options);
    };
    $.ligerui.controls.Button.ligerExtend($.ligerui.controls.Input, {
        __getType: function ()
        {
            return 'Button';
        },
        __idPrev: function ()
        {
            return 'Button';
        },
        _extendMethods: function ()
        {
            return $.ligerMethos.Button;
        },
        _render: function ()
        {
            var g = this, p = this.options;
            g.button = $(g.element);
            g.button.addClass("l-button");
            g.button.append('<div class="l-button-l"></div><div class="l-button-r"></div><span></span>');
            g.button.hover(function () {
                if (p.disabled) return;
                g.button.addClass("l-button-over");
            }, function () {
                if (p.disabled) return;
                g.button.removeClass("l-button-over");
            });
            p.click && g.button.click(function ()
            {
                if (!p.disabled)
                    p.click();
            });
            g.set(p);
        },
        _setIcon : function(url)
        {
            var g = this;
            if (!url)
            {
                g.button.removeClass("l-button-hasicon");
                g.button.find('img').remove();
            } else
            {
                g.button.addClass("l-button-hasicon");
                g.button.append('<img src="' + url + '" />');
            }
        },
        _setEnabled: function (value)
        {
            if (value)
                this.button.removeClass("l-button-disabled");
        },
        _setDisabled: function (value)
        {
            if (value) {
                this.button.addClass("l-button-disabled");
                this.options.disabled = true;
            } else {
                this.button.removeClass("l-button-disabled");
                this.options.disabled = false;
            }
        },
        _setWidth: function (value)
        {
            this.button.width(value);
        },
        _setText: function (value)
        {
            $("span", this.button).html(value);
        },
        setValue: function (value)
        {
            this.set('text', value);
        },
        getValue: function ()
        {
            return this.options.text;
        },
        setEnabled: function ()
        {
            this.set('disabled', false);
        },
        setDisabled: function ()
        {
            this.set('disabled', true);
        }
    }); 


})(jQuery);