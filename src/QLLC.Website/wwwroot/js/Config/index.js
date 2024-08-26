(function () {
    if (typeof Config === 'undefined') {
        Config = {};
    }
    Config.Event = {
        offset: 0,
        limit: 20,
        constructor: function () {
            this.init();
        },

        init() {
          
            this.initKendoGird();
            this.initKendoToolBar();
        },

       

        initKendoGird() {
            $("#configListGrid").kendoGrid({
                dataSource: {
                    transport: {
                        read: {
                            type: "GET",
                            url: "/Configs/GetConfigList"
                        },
                        parameterMap: function (data, type) {
                            if (type == "read") {
                                let search = $("#searchName").length > 0 ? $("#searchName").val() : "";
                                let datas = {
                                    searchString: search,
                                    pageNumber: data.page,
                                    pageSize: data.pageSize
                                }
                               
                                //data.textSearch = search;
                                return datas
                              
                            }

                        },
                    },
                    page: Config.Event.offset,
                    pageSize: Config.Event.limit,
                   
                    serverPaging: true,
                    serverFiltering: true,
                    schema: {
                        parse: function (response) {
                            for (var i = 0; i < response.data.total; i++) {
                               
                                response.data.data[i].stt = i + 1 + (response.data.pageNumber - 1) * response.data.pageSize;
                            }
                            return response.data;
                        },
                        model: {
                            id: "id",
                            fields: {
                                tenantName: { type: "string" },
                                reminderEmailSubject: { type: "string" },
                                stt: { type: "number" }
                            }
                        },
                        data: "data",
                        total: "total",
                    }
                },
                selectable: true,
                toolbar: "<div id='toolbar' style=''  class='w-100 d-flex flex-column'></div>",
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
                    page: Config.Event.offset,
                    pageSize: Config.Event.limit
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
                        field: "tenantName",
                        title: "Tên Chùa",
                        headerAttributes: { style: "text-align: center; justify-content: center;" },
                        attributes: { style: 'text-align: left;' },
                        filterable: false,
                        width: 200
                    },
                    {
                        field: "numberOfDaysNoticeExpiredUrn",
                        title: "Số ngày nhắc hết hạn gửi linh cốt (ngày)",
                        headerAttributes: { style: "text-align: center; justify-content: center;" },
                        attributes: { style: 'text-align: center;' },
                        filterable: false,
                        //template: function (data) {
                        //    var birthDateString = kendo.toString(kendo.parseDate(data.timeNotificationTelegram), "HH:mm")
                         
                        //    return `${birthDateString}`
                        //},
                        width: 300
                    },
                    {
                        field: "numberOfDaysNoticeAnniversary",
                        title: "Số ngày nhắc cúng (ngày)",
                        headerAttributes: { style: "text-align: center; justify-content: center;" },
                        attributes: { style: 'text-align: center;' },
                        filterable: false,
                        width: 220
                    },
                    {
                        field: "remindNotification",
                        title: "Cách bao nhiêu ngày sẽ nhắc lại",
                        headerAttributes: { style: "text-align: center; justify-content: center;" },
                        attributes: { style: 'text-align: center;' },
                        filterable: false,
                        width: 250
                    },
                    {
                        field: "reminderEmailSubject",
                        title: "Nội dung chủ đề",
                        headerAttributes: { style: "text-align: center; justify-content: center;" },
                        attributes: { style: 'text-align: left;' },
                        filterable: false,
                        width: 250
                    },
                    {
                        field: "",
                        title: "Thao tác",
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: 'text-align: center;' },
                        template: "<button onclick='Config.Event.Edit(#:id#)' title='Chỉnh sửa' class='k-button k-button-md k-rounded-md k-button-solid-warning _permission_' data-enum='15'><span class='k-icon k-i-track-changes k-button-icon'></span></button>",
                        width: 100
                    }],

                dataBound: function (e) {
                    CheckPermission();
                }
            });
        },

        initKendoToolBar() {
            //$("#toolbar").kendoToolBar({
            //    items: [
            //        { template: "<label for='searching'>Tìm kiếm:</label>" },
            //        { template: '<input id="searchName" style="width:100%" />' },
            //        { template: '<button id="search" class = "k-button k-button-md k-rounded-md k-button-solid k-button-solid-primary me-2"></button>' },
            //    ]
            //});
            let html = `
                    <div class="row gx-0 row-gap-2 w-100">
                        <div class="col-xl-3 col-lg-4 col-md-4 col-sm-4 col-12">
                                <div class="pe-1">
                                <label for="searchName">Tìm kiếm:</label>
                                <input type="text" class=" w-100" id="searchName"/>
                            </div>
                        </div>
                        <div class="col-xl-3 col-lg-4 col-md-4 col-sm-4 col-12 d-flex align-items-sm-end ">
                            <div class="pe-1">
                                <label for="ds" class=" d-none">Tìm kiếm:</label>
                                <button id="search" title="Tìm kiếm" class = "k-button k-button-md k-rounded-md k-button-solid k-button-solid-primary "></button>
                            </div>
                        </div>
                    </div>
                </div>
        `;
            $("#toolbar").html(html);

            $("#search").kendoButton({
                icon: "search"
            });
            $("#search").click(async function (e) {
                var grid = $(gridId).data("configListGrid");
                grid.dataSource.page(1);
                grid.dataSource.filter({});
                //$("#configListGrid").data("kendoGrid").dataSource.filter({});
            });

            $("#searchName").kendoTextBox({
                icon: {
                    type: "search",
                    position: "end"  // Có thể là "start" hoặc "end"
                },
                placeholder: "Nhập từ khóa tìm kiếm..."
            });

            $("#create").kendoButton({
                icon: "plus"
            });

            $("#export").click(async function (e) {
                let grid = $("#configListGrid").data("kendoGrid");
                grid.saveAsExcel();
            });

          
            $("#create").on('click', function () {
                Config.Event.initformCreateAndEdit();
            });
          

            var grid = $("#configListGrid").data("kendoGrid");
            $(grid.pager?.element).find("[data-role='dropdownlist']").change(function (e) {
                let pagesize = parseInt(e.target.value);

                if (pagesize != undefined && pagesize != Config.Event.limit) {
                    Config.Event.limit = pagesize;
                    Config.Event.offset = 1;
                }
            });

        },

        Edit: function (id) {
            Config.Event.initformCreateAndEdit(titles = "CHỈNH SỬA THÔNG TIN CẤU HÌNH", id = id)
        },

        initformCreateAndEdit(titles = "THÊM MỚI THÔNG TIN CẤU HÌNH", id = "") {

        let myWindow = $("#window");
        $("#window").html("<form id='formCreateAndEdit'></form>");

        let formData = {
            id: "",
            numberOfDaysNoticeExpiredUrn: "",
            numberOfDaysNoticeAnniversary: "",
            remindNotification: "",
            reminderEmailSubject: "",
        };
        let strSubmit = "Thêm";
           

        let element;
            if (id != 0) {
                debugger
                let datas = $("#configListGrid").data("kendoGrid").dataSource.data();
                element = datas.find(e => e.id == Number(id));
                formData.id = element.id;
                formData.numberOfDaysNoticeExpiredUrn = element.numberOfDaysNoticeExpiredUrn;
                formData.numberOfDaysNoticeAnniversary = element.numberOfDaysNoticeAnniversary;
                formData.remindNotification = element.remindNotification;
                formData.reminderEmailSubject = element.reminderEmailSubject;
                strSubmit = "Sửa";
                createForm();
                $("#province").closest('.k-form-field').remove();
               
                
               
            }
            else {
                createForm();
            }

            function createForm() {
                $("#formCreateAndEdit").kendoForm({
                    orientation: "vertical",
                    formData: formData,
                    type: "group",
                    items: [

                        {
                            field: "numberOfDaysNoticeExpiredUrn",
                            title: "Số ngày nhắc hết hạn gửi linh cốt",
                            label: "Số ngày nhắc hết hạn gửi linh cốt (*):",
                            validation: {
                                validationMessage: "Vui lòng điền vào ngày!",
                                required: true
                            },
                            editor: function (container, options) {
                                $('<input id="' + options.field + '" name="' + options.field + '" />')
                                    .appendTo(container)
                                    .kendoNumericTextBox({
                                        min: 1,
                                        step: 1,
                                        format: "0",
                                        value: 1
                                    });
                            }
                        },
                     
                        {
                            field: "numberOfDaysNoticeAnniversary",
                            title: "Số ngày nhắc cúng",
                            label: "Số ngày nhắc cúng (*):",
                            validation: {
                                validationMessage: "Vui lòng điền vào ngày!",
                                required: true
                            },
                            editor: function (container, options) {
                                $('<input id="' + options.field + '" name="' + options.field + '" />')
                                    .appendTo(container)
                                    .kendoNumericTextBox({
                                        min: 1,
                                        step: 1,
                                        format: "0",
                                        value: 1
                                    });
                            }
                        },
                        {
                            field: "remindNotification",
                            title: "Số ngày sẽ nhắc lại",
                            label: "Số ngày sẽ nhắc lại (*):",
                            validation: {
                                validationMessage: "Vui lòng điền vào ngày!",
                                required: true
                            },
                            editor: function (container, options) {
                                $('<input id="' + options.field + '" name="' + options.field + '" />')
                                    .appendTo(container)
                                    .kendoNumericTextBox({
                                        min: 1,
                                        step: 1,
                                        format: "0",
                                        value: 1
                                    });
                            }
                        },
                        {
                            field: "reminderEmailSubject",
                            title: "Nội dung:",
                            label: "Nội dung:",
                            editor: "TextArea",
                            editorOptions: { rows: 2 },

                        },


                    ],
                    messages: {
                        submit: strSubmit, clear: "Xóa"
                    },
                    validateField: function (e) {

                    },
                    submit: function (e) {
                        debugger
                        e.preventDefault();
                        let dataitem = {};
                        let remindNotification_ = $("#remindNotification").val();
                        let reminderEmailSubject_ = $("#reminderEmailSubject").val();
                        let numberOfDaysNoticeExpiredUrn_ = $("#numberOfDaysNoticeExpiredUrn").val();
                        let numberOfDaysNoticeAnniversary_ = $("#numberOfDaysNoticeAnniversary").val();
                     
                        if (id != "") {
                            element.remindNotification = parseInt(remindNotification_);
                            element.numberOfDaysNoticeExpiredUrn = parseInt(numberOfDaysNoticeExpiredUrn_);
                            element.numberOfDaysNoticeAnniversary = parseInt(numberOfDaysNoticeAnniversary_);
                            element.reminderEmailSubject = reminderEmailSubject_;

                            dataitem = element;
                        }
                        else {
                            dataitem = {
                                id: 0,
                                numberOfDaysNoticeExpiredUrn: numberOfDaysNoticeExpiredUrn_,
                                numberOfDaysNoticeAnniversary: numberOfDaysNoticeAnniversary_,
                                remindNotification: remindNotification_,
                                reminderEmailSubject: reminderEmailSubject_,
                            }

                        }

                        $.ajax({
                            url: "/Configs/CreateOrUpdate",
                            type: "POST",
                            datatype: "json",
                            contentType: "application/json",
                            data: JSON.stringify(dataitem),
                            success: function (d) {
                                if (d.isSuccess == true) {

                                    $("#configListGrid").data("kendoGrid").dataSource.filter({});
                                    myWindow.data("kendoWindow").close();
                                    notification.show({
                                        title: "Thông báo",
                                        message: "Lưu thành công!"
                                    }, "success");
                                }
                                else {
                                    notification.show({
                                        title: "Thông báo",
                                        message: d.errorMessageList[0]
                                    }, "error");

                                }
                            }
                        })
                    },
                    close: function (e) {
                        $(this.element).empty();
                    },
                });
            }
           

        function remove() {
            setTimeout(() => {
                if ($(".k-window #window").length > 0) {
                    $("#window").parent().remove();
                    $("#configListGrid").after("<div id='window'></div>");
                }
            }, 200)
        }

        myWindow.kendoWindow({
            width: "500px",
            title: "",
            visible: false,
            actions: ["Close"],
            resizable: false,
            draggable: false,
            modal: true,
            close: function (e) {
                remove();
            },
        }).data("kendoWindow").title(titles).center();
        myWindow.data("kendoWindow").open();
    },

       
       
    }
    Config.Event.constructor();
})(jQuery);
