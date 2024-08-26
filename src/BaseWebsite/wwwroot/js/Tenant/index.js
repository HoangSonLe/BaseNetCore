(function () {
    if (typeof Tenant === 'undefined') {
        Tenant = {};
    }
    Tenant.Event = {
        offset: 0,
        limit: 20,
        dataSourceProvinces: null,
        constructor: function () {
            this.init();
        },

        init() {
          
            this.initKendoGird();
            this.initKendoToolBar();
            this.initDataProvince();
        },

        initDataProvince() {
           $.ajax({
                type: "GET",
               url: "/Provinces/GetListProvince",
                dataType: "json",
               success: function (response) {
                    Tenant.Event.dataSourceProvinces = response;
                }
            });
        },

        initKendoGird() {
            $("#tenantListGrid").kendoGrid({
                dataSource: {
                    transport: {
                        read: {
                            type: "GET",
                            url: "/Tenants/GetTenantList"
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
                    page: Tenant.Event.offset,
                    pageSize: Tenant.Event.limit,
                   
                    serverPaging: true,
                    serverFiltering: true,
                    schema: {
                        parse: function (response) {
                            //for (var i = 0; i < response.data.length; i++) {
                               
                            //    response.data.data[i].stt = i + 1 + (response.data.pageNumber - 1) * response.data.pageSize;
                            //}
                            return response.data;
                        },
                        model: {
                            id: "id",
                            fields: {
                                name: { type: "string" },
                                code: { type: "string" },
                                address: { type: "string" },
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
                    page: Tenant.Event.offset,
                    pageSize: Tenant.Event.limit
                },
                dataBinding: function (e) {
                    record = (this.dataSource._page - 1) * this.dataSource._pageSize;
                },
                columns: [
                    {
                        field: "stt",
                        title: "STT",
                        filterable: false,
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: "text-align: center; justify-content: center;" },
                        template: "#: ++record #",
                        width: 100,
                    },
                    {
                        field: "name",
                        title: "Tên Chùa",
                        headerAttributes: { style: "text-align: center; justify-content: center;" },
                        attributes: { style: 'text-align: left;' },
                        filterable: false,
                        width: 300
                    },
                    {
                        field: "code",
                        title: "Mã code",
                        headerAttributes: { style: "text-align: center; justify-content: center;" },
                        filterable: false,
                        width: 120
                    },
                    {
                        field: "address",
                        title: "Địa chỉ",
                        headerAttributes: { style: "text-align: center; justify-content: center;" },
                        attributes: { style: 'text-align: left;' },
                        filterable: false,
                        width: 200
                    },
                    {
                        field: "",
                        title: "Thao tác",
                        headerAttributes: { style: "text-align: center; justify-content: center" },
                        attributes: { style: 'text-align: center;' },
                        template: "<button style='margin-right:10px;' onclick='Tenant.Event.Edit(#:id#)' title='Chỉnh sửa' class='k-button k-button-md k-rounded-md k-button-solid-warning _permission_' data-enum='12' ><span class='k-icon k-i-track-changes k-button-icon'></span></button>\
                                   <button onclick='Tenant.Event.DeleteTenant(#:id#)' title='Xoá' class='k-button k-button-md k-rounded-md k-button-solid-error  _permission_' data-enum='13'><span class='k-icon k-i-trash k-button-icon'></span></button>",
                        width: 140
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
            //        { template: '<button id="create" class="k-button k-button-md k-rounded-md k-button-solid k-button-solid-success  mb-sm-2 me-sm-2 me-2 position-sm-absolute end-0  position-relative _permission_" data-enum= "10" style="position: absolute !important;right: 5px;">Thêm</button>' },
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
                                <button id="search" title="Tìm kiếm" class = "k-button k-button-md k-rounded-md k-button-solid k-button-solid-primary "></button>
                                <button id="create" title="Thêm"  class="k-button k-button-md k-rounded-md k-button-solid k-button-solid-success _permission_" data-enum='10'><span class='k-icon k-i-plus k-button-icon'></span><span class='k-button-text'>Thêm</span></button>
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
                var grid = $(gridId).data("tenantListGrid");
                grid.dataSource.page(1);
                grid.dataSource.filter({});
                //$("#tenantListGrid").data("kendoGrid").dataSource.filter({});
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
                let grid = $("#tenantListGrid").data("kendoGrid");
                grid.saveAsExcel();
            });

          
            $("#create").on('click', function () {
                Tenant.Event.initformCreateAndEdit();
            });
          

            var grid = $("#tenantListGrid").data("kendoGrid");
            $(grid.pager?.element).find("[data-role='dropdownlist']").change(function (e) {
                let pagesize = parseInt(e.target.value);

                if (pagesize != undefined && pagesize != Tenant.Event.limit) {
                    Tenant.Event.limit = pagesize;
                    Tenant.Event.offset = 1;
                }
            });

        },

        Edit: function (id) {
            Tenant.Event.initformCreateAndEdit(titles = "CHỈNH SỬA THÔNG TIN CHÙA", id = id)
        },

        initformCreateAndEdit(titles = "THÊM MỚI CHÙA", id = "") {

        let myWindow = $("#window");
        $("#window").html("<form id='formCreateAndEdit'></form>");

        let formData = {
            id: "",
            name: "",
            code: "",
            address: "",
        };
        let strSubmit = "Thêm";
           

        let element;
            if (id != 0) {
                
                let datas = $("#tenantListGrid").data("kendoGrid").dataSource.data();
                element = datas.find(e => e.id == Number(id));
                formData.id = element.id;
                formData.name = element.name;
                formData.code = element.code;
                formData.address = element.address;
                strSubmit = "Sửa";
                createForm();
                $("#province").closest('.k-form-field').remove();
                $("#name").closest('.k-form-field').addClass("k-disabled");
                $("#code").closest('.k-form-field').addClass("k-disabled");
               
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
                            field: "name",
                            title: "Tên chùa",
                            label: "Tên chùa (*):",
                            validation: {
                                validationMessage: "Vui lòng nhập tên chùa",
                                required: true
                            },
                        },
                        {
                            field: "province",
                            title: "Tỉnh thành",
                            label: "Tỉnh thành (*):",
                            editor: "DropDownList",
                            editorOptions: {
                                dataSource: {
                                    data: Tenant.Event.dataSourceProvinces,
                                },
                                optionLabel: "Chọn Tỉnh/TP",
                                dataTextField: "name",
                                dataValueField: "code",
                                filter: filterCustom,
                                change: function () {
                                    $("#code").val($("#province").val())
                                },
                            },
                            validation: { required: true, validationMessage: "Vui lòng chọn tỉnh/thành phố" },
                        },
                        {
                            field: "code",
                            label: "Code: ",
                            title: "Code",
                            //attributes: { class: 'k-disabled' },
                            //validation: { required: true, validationMessage: "Vui lòng nhập mã code" },

                        },
                        {
                            field: "address",
                            title: "Địa chỉ:",
                            label: "Địa chỉ:",
                            editor: "TextArea",
                            editorOptions: { rows: 2 },

                        },


                    ],
                    messages: {
                        submit: strSubmit
                    },
                    validateField: function (e) {

                    },
                    submit: function (e) {
                        e.preventDefault();
                        let dataitem = {};
                        let address_ = $("#address").val();
                        let name_ = $("#name").val();
                        let code_ = $("#code").val();
                        if (id != "") {
                            element.address = address_;
                            element.name = name_;
                            element.code = code_;

                            dataitem = element;
                        }
                        else {
                            dataitem = {
                                id: 0,
                                name: name_,
                                code: code_,
                                address: address_,
                            }

                        }

                        $.ajax({
                            url: "/Tenants/CreateOrUpdate",
                            type: "POST",
                            datatype: "json",
                            contentType: "application/json",
                            data: JSON.stringify(dataitem),
                            success: function (d) {
                                if (d.isSuccess == true) {

                                    $("#tenantListGrid").data("kendoGrid").dataSource.filter({});
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
                $("#code").closest('.k-form-field').addClass("k-disabled");
                $(".k-form-clear").hide();
            }
           
        function remove() {
            setTimeout(() => {
                if ($(".k-window #window").length > 0) {
                    $("#window").parent().remove();
                    $("#tenantListGrid").after("<div id='window'></div>");
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

        DeleteTenant: function (id) {
            if (id <= 0 || isNaN(id))
                notification.show({
                    title: "Thông báo",
                    message: "Vui lòng chọn chùa!"
                }, "error");

            $("#NotificationDelete").html("<form id='myFormNotiDelete'></form>");
            $("#myFormNotiDelete").kendoForm({
                orientation: "horizontal",

                messages: {
                    submit: "Xác nhận", clear: "Hủy bỏ"
                },
                clear: async function (e) {
                    var dialog = $("#NotificationDelete").data("kendoWindow");
                    setTimeout(function () {
                        dialog.close();
                    }, 100);
                },
                submit: async function (e) {
                    e.preventDefault();
                    var dialog = $("#NotificationDelete").data("kendoWindow");
                    setTimeout(function () {
                        dialog.close();
                    }, 100);

                    $.ajax({
                        url: "/Tenants/DeleteTenantById",
                        data: {
                            tenantId: id,
                        },
                        success: function (data) {
                            if (data.isSuccess == true) {
                                
                                notification.show({
                                    title: "Thông báo",
                                    message: "Xóa chùa thành công"
                                }, "success");
                                $("#tenantListGrid").data("kendoGrid").dataSource.filter({});
                            }
                            else
                                notification.show({
                                    title: "Thông báo",
                                    message: data.errorMessageList
                                }, "error");
                        },
                        error: function (data) {
                            notification.show({
                                title: "Thông báo",
                                message: "Xóa chùa thất bại!"
                            }, "error");
                        }
                    });
                },
                close: function (e) {
                    $(this.element).empty();
                },
            });
            $("#NotificationDelete").kendoWindow({
                width: "450px",
                title: "Thông báo xác nhận xóa chùa ?",
                visible: false,
                modal: true,
                actions: [
                    "Close"
                ],
            }).data("kendoWindow").center().open();

           // $("#myFormNotiDelete .k-form-buttons.k-buttons-end").removeClass("k-buttons-end");
        }, 
       
    }
    Tenant.Event.constructor();
})(jQuery);
