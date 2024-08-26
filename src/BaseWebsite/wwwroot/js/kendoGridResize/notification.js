
var notification = $("#notification").kendoNotification({
    position: {
        pinned: true,
        bottom: 30,
        right: 30
    },
    autoHideAfter: 2000,
    stacking: "top",
    animation: {
        open: {
            effects: "slideIn:left"
        },
        close: {
            effects: "slideIn:left",
            reverse: true
        }
    },
    templates: [
        {
            type: "info",
            template: $("#wrongTemplate").html()
        },
        {
            type: "error",
            template: $("#errorTemplate").html()
        },
        {
            type: "success",
            template: $("#successTemplate").html()
        }
        //{
        //    type: "warning",
        //    template: $("#warningTemplate").html()
        //}
    ]

}).data("kendoNotification");