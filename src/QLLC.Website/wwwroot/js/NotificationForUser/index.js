(function () {
    if (typeof NotificationForUser === 'undefined') {
        NotificationForUser = {};
    }
    NotificationForUser = {
        now: new Date(),
        pageNumber: 1,
        pageSize: 99999999,
        dataSource: null,
        constructor: function () {
            if (Userdata.roleIdList.find(e => e == ERoleType.Admin || e == ERoleType.Reporter))
                this.init();
        },

        init() {
            if (localStorage.getItem("QLLC_data_noti") != null) {
                let QLLC_data_noti = JSON.parse(localStorage.getItem("QLLC_data_noti"));
                if (QLLC_data_noti.date == kendo.toString(new Date(), "dd/MM/yyyy") && QLLC_data_noti.userId == Userdata?.id) return;
            }

            this.initTabstrip();
        },
        initTabstrip() {
            let body = document.querySelector("body");
            let PopupId = "#divPopup";
            let divPopup = null;
            if ($(divPopup).length == 0) {
                divPopup = document.createElement("div");
                divPopup.id = "divPopup";
                body.append(divPopup);
            }
            else {
                divPopup = document.querySelector(PopupId);
            }

            let divTapStrip = document.createElement("div");
            let html = `
            <div id="tapStrip" class="">
                <ul>
                    <li class="k-active px-3 ngaycung" >
                        Đến ngày cúng
                    </li>
                    <li class="px-3 hankygui ">
                        Hết hạn ký gửi
                    </li>
                </ul>
                <div>
                    <div id="gridIdWorshipDay" class=""></div>
                </div>

                <div>
                    <div id="gridIdConsignmentExpired" class=""></div>
                </div>
            </div>`;
            divTapStrip.innerHTML = html;
            divPopup.append(divTapStrip);

            this.initTapStrip("#tapStrip")
            this.initKendoGirdWorshipDay("#gridIdWorshipDay");
            this.initKendoConsignmentExpired("#gridIdConsignmentExpired");
            //$(".hankygui").append(`<span id="badge-inside" data-role="badge" class="k-badge k-badge-solid k-badge-solid-primary k-badge-md k-rounded-md k-badge-inside k-top-end">10</span>`)// so dong 
            this.initShowWindow(PopupId);
        },
        initShowWindow(divPopup, titles = "Thông báo") {
            let myWindow = $(divPopup);
            myWindow.kendoWindow({
                width: "80%",
                minHeight: "80%",
                maxHeight: "100%",
                title: "",
                visible: false,
                actions: ["Maximize", "Close"],
                resizable: false,
                draggable: false,
                modal: true,
                close: function (e) {
                    //$("#window").empty();
                    // remove();
                    let data = {
                        date: kendo.toString(new Date(), "dd/MM/yyyy"),
                        userId: Userdata.id
                    }
                    localStorage.setItem("QLLC_data_noti", JSON.stringify(data));
                },
            }).data("kendoWindow").title(titles).center();
            myWindow.data("kendoWindow").open();

            $("#tapStrip").height($(divPopup).height());
        },
        initTapStrip(tabStripId) {
            $(tabStripId).kendoTabStrip({
                animation: {
                    open: {
                        effects: "fadeIn"
                    }
                }
            }).data("kendoTabStrip").select(0);
        },
        initKendoGirdWorshipDay(gridId) {
            $(gridId).kendoGrid({
                dataSource: {
                    transport: {
                        read: {
                            type: "GET",
                            url: "/Urn/GetUrnWorshipDayList"
                        },
                        parameterMap: function (data, type) {
                            if (type == "read") {
                                //let search = $("#searchName").length > 0 ? $("#searchName").val() : "";
                                let datas = {
                                    //searchString: search,
                                    pageNumber: data.page,
                                    pageSize: data.pageSize
                                }

                                //data.textSearch = search;
                                return datas

                            }

                        },
                    },
                    page: NotificationForUser.pageNumber,
                    pageSize: NotificationForUser.pageSize,

                    //serverPaging: true,
                    //serverFiltering: true,
                    schema: {
                        parse: function (response) {
                            let datas = response.data.data;
                            let pageNumber_ = response.data.pageNumber;
                            let pageSize_ = response.data.pageSize;
                            for (var i = 0; i < datas.length; i++) {

                                datas[i].stt = (i + 1) + (pageNumber_ - 1) * pageSize_;
                            }
                            return response.data;
                        },
                        model: {
                            id: "id",
                            fields: {
                                stt: { type: "number" },
                                birthDate: { type: "date" },
                                deathDate: { type: "date" },
                                expiredDate: { type: "date" },

                            }
                        },
                        data: "data",
                        total: "total",
                    }
                },
                height: "99%",
                //toolbar: ["excel"],
                //excel: {
                //    fileName: "Danh sách linh cốt sắp đến ngày cúng " + kendo.toString(NotificationForUser.now, "dd_MM_yyyy")+".xlsx",
                //    allPages: true,
                //    filterable: true
                //},
                //excelExport: function (e) {
                //    var sheet = e.workbook.sheets[0];
                //    for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                //        var row = sheet.rows[rowIndex];
                //        debugger
                //        for (var cellIndex = 2; cellIndex < row.cells.length; cellIndex++) {
                //            if ((row.type == "group-footer" || row.type == "footer") && cellIndex >= 3) {
                //                row.cells[cellIndex].value = $(row.cells[cellIndex].value).text();
                //            }
                //            if (Number.isInteger(Number.parseInt(row.cells[cellIndex].value))) {
                //                if (row.type == "group-footer" || row.type == "footer") {
                //                    row.cells[cellIndex].hAlign = "right";
                //                    row.cells[cellIndex].format = "[red]0";
                //                } else
                //                    row.cells[cellIndex].format = "[Blue]0";
                //            }
                //        }
                //    } 

                //    var columns = sheet.columns;
                //    columns.forEach(function (column) {
                //        delete column.width;
                //        column.autoWidth = true;
                //    });

                //    //let tileExcel = "Báo cáo thống kê lưu lượng_" + kendo.toString(new Date(), "dd_MM_yyyy") + ".xlsx";
                //    //e.workbook.fileName = tileExcel;
                //},
                selectable: true,
                scrollable: {
                    virtual: true
                },
                sortable: true,
                sort: function (e) {
                },
                toolbar: `<div id='toolbar' style='width:99.47%'>
                <input id="txtSearchWorshipDay" class=" me-2"  />
                <button id='searchWorshipDay' class='k-button k-button-md k-rounded-md k-button-solid k-button-solid-primary k-icon-button me-2 mb-sm-0 mb-2' ><span class='k-icon k-i-search k-button-icon'></span><span class='k-button-text d-none'>Tìm kiếm</span></button>
                <button id='exportExcelWorshipDay' class='k-button k-button-md k-rounded-md k-button-outline k-button-outline-error  mb-sm-0 mb-2 ' style=''><span class='k-icon k-i-file-excel k-button-icon'></span><span class='k-button-text'>Export Excel</span></button>
                </div>`,
                pageable: {
                    messages: {
                        display: "{0} - {1} của {2} dòng", // {0} is the index of the first record on the page, {1} - the index of the last record on the page, {2} is the total amount of records.
                        empty: "Không tìm thấy dữ liệu",
                        page: "Trang",
                        allPages: "Tất cả",
                        of: "của {0}", // {0} is total amount of pages.
                        itemsPerPage: "",
                        first: "",
                        previous: "Lùi",
                        next: "Tiếp theo",
                        last: "Cuối",
                        refresh: "Làm mới"
                    },
                    pageSizes: [10, 20, 50],
                    pageSize: 20
                },
                columns: [
                    {
                        field: "stt",
                        title: "STT",
                        filterable: false,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center; justify-content: center;" },
                        width: 100,
                    },
                    {
                        field: "name",
                        title: "Thông tin",
                        headerAttributes: { style: "text-align: center; justify-content: center;" },
                        attributes: { style: 'text-align: left;' },
                        filterable: false,
                        width: 300,
                        template: function (data) {
                            return `<div>
                                    <div class="infor-container">
                                        <div class="title">Họ tên:</div>
                                        <div class="content">${data.name}</div>
                                    </div>
                                    <div class="infor-container">
                                        <div class="title">Pháp danh:</div>
                                        <div class="content">${data.dharmaName}</div>
                                    </div>
                                    <div class="infor-container">
                                        <div class="title">Giới tính:</div>
                                        <div class="content">${data.genderName}</div>
                                    </div>
                               </div>`
                        }
                    },
                    {
                        field: "birthDate",
                        title: "Ngày sinh - mất (Ngày âm)",
                        width: 200,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align:center;" },
                        template: function (data) {
                            var birthDateString = kendo.toString((data.birthDate), "dd/MM/yyyy")
                            var deathDateString = kendo.toString((data.deathDate), "dd/MM/yyyy")
                            return `${birthDateString} - ${deathDateString}`
                        },
                    },
                    {
                        field: "location",
                        title: "Vị trí linh cốt",
                        width: 300,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        template: function (data) {
                            return `<div>
                                        <div class="infor-container">
                                            <div class="title">Vị trí tháp:</div>
                                            <div class="content">${data.towerLocation}</div>
                                        </div>
                                        <div class="infor-container">
                                            <div class="title">Tên tủ:</div>
                                            <div class="content">${data.cabinetName}</div>
                                        </div>
                                        <div class="infor-container">
                                            <div class="title">Số hàng:</div>
                                            <div class="content">${data.rowNumber}</div>
                                        </div>
                                         <div class="infor-container">
                                            <div class="title">Ô số:</div>
                                            <div class="content">${data.boxNumber}</div>
                                        </div>
                                   </div>`
                        }
                    },
                    {
                        field: "note",
                        title: "Ghi chú",
                        width: 300,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                    },
                ],
            });

            $("#txtSearchWorshipDay").kendoTextBox({
                placeholder: "Từ khoá tìm kiếm..."
            });
            $("#txtSearchWorshipDay").data("kendoTextBox").wrapper.addClass("mb-sm-0 mb-2")

            $("#searchWorshipDay").on('click', function () {
                let searchKey = $("#txtSearchWorshipDay").val();
                let filter = {
                    logic: "or",
                    filters: [
                        {
                            field: "name",
                            operator: "contains",
                            value: searchKey
                        },
                        {
                            field: "genderName",
                            operator: "contains",
                            value: searchKey
                        },
                        {
                            field: "dharmaName",
                            operator: "contains",
                            value: searchKey
                        },
                        {
                            field: "towerLocation",
                            operator: "contains",
                            value: searchKey
                        },
                        {
                            field: "cabinetName",
                            operator: "contains",
                            value: searchKey
                        },
                        {
                            field: "note",
                            operator: "contains",
                            value: searchKey
                        },
                        //{
                        //    field: "boxNumber",
                        //    operator: "contains",
                        //    value: searchKey
                        //},
                        //{
                        //    field: "genderName",
                        //    operator: "contains",
                        //    value: searchKey
                        //},
                    ]
                };
                NotificationForUser.filterOffline(gridId, filter);
            });
            $("#exportExcelWorshipDay").click(function (e) {
                //let grid = $(gridId).data("kendoGrid");
                //grid.saveAsExcel();
                NotificationForUser.exportExcelWorshipDay();
            });
        },
        initKendoConsignmentExpired(gridId) {
            $(gridId).kendoGrid({
                dataSource: {
                    transport: {
                        read: {
                            type: "GET",
                            url: "/Urn/GetUrnConsignmentExpired"
                        },
                        parameterMap: function (data, type) {
                            if (type == "read") {
                                //let search = $("#searchName").length > 0 ? $("#searchName").val() : "";
                                let datas = {
                                    //searchString: search,
                                    pageNumber: data.page,
                                    pageSize: data.pageSize
                                }

                                //data.textSearch = search;
                                return datas

                            }

                        },
                    },
                    page: NotificationForUser.pageNumber,
                    pageSize: NotificationForUser.pageSize,

                    //serverPaging: true,
                    //serverFiltering: true,
                    schema: {
                        parse: function (response) {
                            let datas = response.data.data;
                            let pageNumber_ = response.data.pageNumber;
                            let pageSize_ = response.data.pageSize;
                            for (var i = 0; i < datas.length; i++) {

                                datas[i].stt = (i + 1) + (pageNumber_ - 1) * pageSize_;
                            }
                            return response.data;
                        },
                        model: {
                            id: "id",
                            fields: {
                                stt: { type: "number" },
                                birthDate: { type: "date" },
                                deathDate: { type: "date" },
                                expiredDate: { type: "date" },
                            }
                        },
                        data: "data",
                        total: "total",
                    }
                },
                height: "99%",
                //toolbar: ["excel"],
                //excel: {
                //    fileName: "Danh sách linh cốt hết hạn ký gửi " + kendo.toString(NotificationForUser.now, "dd_MM_yyyy") + ".xlsx",
                //    allPages: true,
                //    filterable: true
                //},
                //excelExport: function (e) {
                //    var sheet = e.workbook.sheets[0];
                //    for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                //        var row = sheet.rows[rowIndex];
                //        for (var cellIndex = 2; cellIndex < row.cells.length; cellIndex++) {
                //            if ((row.type == "group-footer" || row.type == "footer") && cellIndex >= 3) {
                //                row.cells[cellIndex].value = $(row.cells[cellIndex].value).text();
                //            }
                //            if (Number.isInteger(Number.parseInt(row.cells[cellIndex].value))) {
                //                if (row.type == "group-footer" || row.type == "footer") {
                //                    row.cells[cellIndex].hAlign = "right";
                //                    row.cells[cellIndex].format = "[red]0";
                //                } else
                //                    row.cells[cellIndex].format = "[Blue]0";
                //            }
                //        }
                //    }

                //    var columns = sheet.columns;
                //    columns.forEach(function (column) {
                //        delete column.width;
                //        column.autoWidth = true;
                //    });

                //    //let tileExcel = "Báo cáo thống kê lưu lượng_" + kendo.toString(new Date(), "dd_MM_yyyy") + ".xlsx";
                //    //e.workbook.fileName = tileExcel;
                //},
                selectable: true,
                scrollable: {
                    virtual: true
                },
                sortable: true,
                sort: function (e) {
                },
                toolbar: `<div id='toolbar' style='width:99.47%'>
                <input id="txtSearchConsignmentExpired" class=" me-2"  />
                <button id='searchConsignmentExpired' class='k-button k-button-md k-rounded-md k-button-solid k-button-solid-primary k-icon-button me-2 mb-sm-0 mb-2' ><span class='k-icon k-i-search k-button-icon'></span><span class='k-button-text d-none'>Tìm kiếm</span></button>
                <button id='exportExcelConsignmentExpired' class='k-button k-button-md k-rounded-md k-button-outline k-button-outline-error  mb-sm-0 mb-2 ' style=''><span class='k-icon k-i-file-excel k-button-icon'></span><span class='k-button-text'>Export Excel</span></button>
                </div>`,
                pageable: {
                    messages: {
                        display: "{0} - {1} của {2} dòng", // {0} is the index of the first record on the page, {1} - the index of the last record on the page, {2} is the total amount of records.
                        empty: "Không tìm thấy dữ liệu",
                        page: "Trang",
                        allPages: "Tất cả",
                        of: "của {0}", // {0} is total amount of pages.
                        itemsPerPage: "",
                        first: "",
                        previous: "Lùi",
                        next: "Tiếp theo",
                        last: "Cuối",
                        refresh: "Làm mới"
                    },
                    pageSizes: [10, 20, 50],
                    pageSize: 20
                },
                columns: [
                    {
                        field: "stt",
                        title: "STT",
                        filterable: false,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center; justify-content: center;" },
                        width: 100,
                    },
                    {
                        field: "name",
                        title: "Thông tin",
                        headerAttributes: { style: "text-align: center; justify-content: center;" },
                        attributes: { style: 'text-align: left;' },
                        filterable: false,
                        width: 300,
                        template: function (data) {
                            return `<div>
                                    <div class="infor-container">
                                        <div class="title">Họ tên:</div>
                                        <div class="content">${data.name}</div>
                                    </div>
                                    <div class="infor-container">
                                        <div class="title">Pháp danh:</div>
                                        <div class="content">${data.dharmaName}</div>
                                    </div>
                                    <div class="infor-container">
                                        <div class="title">Giới tính:</div>
                                        <div class="content">${data.genderName}</div>
                                    </div>
                               </div>`
                        }
                    },
                    {
                        field: "expiredDate",
                        title: "Thời hạn ký gửi",
                        width: 150,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align:center;" },
                        template: '#: kendo.toString((expiredDate), "dd/MM/yyyy")#',
                    },
                    {
                        field: "location",
                        title: "Vị trí linh cốt",
                        width: 300,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        template: function (data) {
                            return `<div>
                                        <div class="infor-container">
                                            <div class="title">Vị trí tháp:</div>
                                            <div class="content">${data.towerLocation}</div>
                                        </div>
                                        <div class="infor-container">
                                            <div class="title">Tên tủ:</div>
                                            <div class="content">${data.cabinetName}</div>
                                        </div>
                                        <div class="infor-container">
                                            <div class="title">Số hàng:</div>
                                            <div class="content">${data.rowNumber}</div>
                                        </div>
                                         <div class="infor-container">
                                            <div class="title">Ô số:</div>
                                            <div class="content">${data.boxNumber}</div>
                                        </div>
                                   </div>`
                        }
                    },
                    {
                        field: "note",
                        title: "Ghi chú",
                        width: 300,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                    },
                ],
            });


            $("#txtSearchConsignmentExpired").kendoTextBox({
                placeholder: "Từ khoá tìm kiếm..."
            });
            $("#txtSearchConsignmentExpired").data("kendoTextBox").wrapper.addClass("mb-sm-0 mb-2")

            $("#searchConsignmentExpired").on('click', function () {
                let searchKey = $("#txtSearchConsignmentExpired").val();
                let filter = {
                    logic: "or",
                    filters: [
                        {
                            field: "name",
                            operator: "contains",
                            value: searchKey
                        },
                        {
                            field: "genderName",
                            operator: "contains",
                            value: searchKey
                        },
                        {
                            field: "dharmaName",
                            operator: "contains",
                            value: searchKey
                        },
                        {
                            field: "towerLocation",
                            operator: "contains",
                            value: searchKey
                        },
                        {
                            field: "cabinetName",
                            operator: "contains",
                            value: searchKey
                        },
                        {
                            field: "note",
                            operator: "contains",
                            value: searchKey
                        },
                        //{
                        //    field: "boxNumber",
                        //    operator: "contains",
                        //    value: searchKey
                        //},
                        //{
                        //    field: "genderName",
                        //    operator: "contains",
                        //    value: searchKey
                        //},
                    ]
                };
                NotificationForUser.filterOffline(gridId, filter);
            });
            $("#exportExcelConsignmentExpired").click(function (e) {
                //let grid = $(gridId).data("kendoGrid");
                //grid.saveAsExcel();
                NotificationForUser.exportExcelConsignmentExpired();
            });
        },
        exportExcelWorshipDay(grid) {
            let datas = {
                pageNumber: NotificationForUser.pageNumber,
                pageSize: 99999999,
            }
            let height = 100;
            $.ajax({
                type: "GET",
                url: "/Urn/GetUrnWorshipDayList",
                data: (datas),
                success: function (data) {
                    debugger
                    let dataSheet1 = [
                        {
                            cells: [
                                {
                                    value: "STT", textAlign: "center", background: "#428dd8"
                                },
                                {
                                    value: "Thông tin", textAlign: "center", background: "#428dd8"
                                },
                                {
                                    value: "Ngày sinh - mất (Ngày âm)", textAlign: "center", background: "#428dd8"
                                },
                                {
                                    value: "Vị trí linh cốt", textAlign: "center", background: "#428dd8"
                                },
                                {
                                    value: "Ghi chú", textAlign: "center", background: "#428dd8"
                                }
                            ]
                        }];
                    let dataSoureSheet1 = data.data.data;
                    for (let i1 = 0; i1 < dataSoureSheet1.length; i1++) {
                        dataSheet1.push({
                            cells: [
                                {
                                    value: (i1 + 1),
                                    wrap: true,
                                    verticalAlign: "top"
                                },
                                {
                                    value: `Họ tên: ${dataSoureSheet1[i1].name}\nPháp danh: ${dataSoureSheet1[i1].dharmaName}\nGiới tính: ${dataSoureSheet1[i1].genderName}\n`,
                                    wrap: true,
                                    verticalAlign: "top",
                                },
                                {
                                    value: `${kendo.toString(kendo.parseDate(dataSoureSheet1[i1].birthDate), "dd/MM/yyyy")} - ${kendo.toString(kendo.parseDate(dataSoureSheet1[i1].deathDate), "dd/MM/yyyy")}`,
                                    verticalAlign: "top",
                                    textAlign: "center"
                                },
                                {
                                    value: `Vị trí tháp: ${dataSoureSheet1[i1].towerLocation}\nTên tủ: ${dataSoureSheet1[i1].cabinetName}\nSố hàng: ${dataSoureSheet1[i1].rowNumber}\nÔ số: ${dataSoureSheet1[i1].boxNumber}\n`,
                                    wrap: true,
                                    verticalAlign: "top",
                                },
                                {
                                    value: dataSoureSheet1[i1].note,
                                    wrap: true,
                                    verticalAlign: "top"
                                },
                            ],

                            height: height,
                        })
                    }

                    var workbook = new kendo.ooxml.Workbook({
                        sheets: [
                            {
                                name: "Sheet",
                                columns: [{ autoWidth: true }, { autoWidth: true }, { autoWidth: true }, { autoWidth: true }, { autoWidth: true }],
                                rows: dataSheet1,
                            },
                        ],
                    });
                    kendo.saveAs({
                        dataURI: workbook.toDataURL(),
                        fileName: "Danh sách linh cốt sắp đến ngày cúng " + kendo.toString(NotificationForUser.now, "dd_MM_yyyy") + ".xlsx"
                    });
                }
            })

        },
        exportExcelConsignmentExpired() {
            let datas = {
                pageNumber: NotificationForUser.pageNumber,
                pageSize: 99999999,
            }
            let height = 100;
            $.ajax({
                type: "GET",
                url: "/Urn/GetUrnConsignmentExpired",
                data: (datas),
                success: function (data) {
                    
                    let dataSheet1 = [
                        {
                            cells: [
                                {
                                    value: "STT", textAlign: "center", background: "#428dd8"
                                },
                                {
                                    value: "Thông tin", textAlign: "center", background: "#428dd8"
                                },
                                {
                                    value: "Thời hạn ký gửi", textAlign: "center", background: "#428dd8"
                                },
                                {
                                    value: "Vị trí linh cốt", textAlign: "center", background: "#428dd8"
                                },
                                {
                                    value: "Ghi chú", textAlign: "center", background: "#428dd8"
                                }
                            ]
                        }];
                    let dataSoureSheet1 = data.data.data;
                    for (let i1 = 0; i1 < dataSoureSheet1.length; i1++) {
                        dataSheet1.push({
                            cells: [
                                {
                                    value: (i1 + 1),
                                    wrap: true,
                                    verticalAlign: "top"
                                },
                                {
                                    value: {
                                        text: `Họ tên: ${dataSoureSheet1[i1].name}\nPháp danh: ${dataSoureSheet1[i1].dharmaName}\nGiới tính: ${dataSoureSheet1[i1].genderName}\n`,
                                        encoded: true
                                    },
                                    wrap: true,
                                    verticalAlign: "top",
                                },
                                {
                                    value: `${kendo.toString(kendo.parseDate(dataSoureSheet1[i1].expiredDate), "dd/MM/yyyy")}`,
                                    verticalAlign: "top",
                                    textAlign: "center"
                                },
                                {
                                    value: `Vị trí tháp: ${dataSoureSheet1[i1].towerLocation}\nTên tủ: ${dataSoureSheet1[i1].cabinetName}\nSố hàng: ${dataSoureSheet1[i1].rowNumber}\nÔ số: ${dataSoureSheet1[i1].boxNumber}\n`,
                                    wrap: true,
                                    verticalAlign: "top",
                                },
                                {
                                    value: dataSoureSheet1[i1].note,
                                    wrap: true,
                                    verticalAlign: "top"
                                },
                            ],

                            height: height,
                        })
                    }

                    var workbook = new kendo.ooxml.Workbook({
                        sheets: [
                            {
                                name: "Sheet",
                                columns: [{ autoWidth: true }, { autoWidth: true }, { autoWidth: true }, { autoWidth: true }, { autoWidth: true }],
                                rows: dataSheet1,
                            },
                        ],
                    });
                    
                    kendo.saveAs({
                        dataURI: workbook.toDataURL(),
                        fileName: "Danh sách linh cốt hết hạn ký gửi " + kendo.toString(NotificationForUser.now, "dd_MM_yyyy") + ".xlsx",
                    });
                }
            })
        },
        filterOffline(gridId, filter) {
            $(gridId).data("kendoGrid").dataSource.filter(filter);
        }
    }
})(jQuery);