function ResizeGrid(gridId="") {

    var gridElement = gridId == "" ? $("#gridId") : $("#" + gridId),
        newHeight = gridElement.innerHeight(),
        otherElements = gridElement.children().not(".k-grid-content"),
        otherElementsHeight = 0;

    otherElements.each(function () {
        otherElementsHeight += $(this).outerHeight();
    });

    gridElement.children(".k-grid-content").height(newHeight - otherElementsHeight);
}
function FixHeightGrid(gridId = "") {
    var gridElement = gridId == "" ? $("#gridId") : $("#" + gridId);
    //gridElement.height($("main").outerHeight() - $("main div.title").outerHeight());
    gridElement.height($("main").height() - $("main h4.title").height() - 32 -8);
}
$(window).resize(function () {
    FixHeightGrid();
    ResizeGrid();
});