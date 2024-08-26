$(document).ready(function () {
    var formPassword = $("#FormChangePassword");

    if (typeof kendoChangePasswordWindow === 'undefined') {
        initChangPasswordKendoWindow("Đổi mật khẩu", '600px');
    }

    function openChangPasswordKendoWindow () {
        if (kendoChangePasswordWindow) {
            kendoChangePasswordWindow.data("kendoWindow").center().open();
        }
    };

    function initFormChangePassword() {
        if (typeof formChangePassword !== 'undefined') {
            formChangePassword.clear();
            return;
        }

        formChangePassword = formPassword.kendoForm({
            orientation: "vertical",
            formData: {
                oldPassword: "",
                newPassword: "",
                repeatPassword: ""
            },
            validatable: {
                validateOnBlur: true,
                validationSummary: true,
                //errorTemplate: "<span>#=message#</span>"
            },
            layout: "grid",
            type: "group",
            items: [
                {
                    field: "oldPassword",
                    label: "Mật khẩu cũ",
                    title: "Mật khẩu cũ",
                    validation: { required: false, type: 'password' }
                },
                {
                    field: "newPassword",
                    label: "Mật khẩu mới",
                    title: "Mật khẩu mới",
                    validation: { required: false, type: 'password' }
                },
                {
                    field: "repeatPassword",
                    label: "Xác nhận mật khẩu",
                    title: "Xác nhận mật khẩu",
                    validation: { required: false, type: 'password' }
                }
            ],
            messages: {
                submit: "Đổi mật khẩu", clear: "Nhập lại"
            },
        }).getKendoForm();

        formPassword.kendoValidator({
            messages: {
                requiredOldPassword: "Vui lòng nhập vào mật khẩu cũ",
                requiredNewPassword: "Vui lòng nhập vào mật khẩu mới",
                requiredRepeatPassword: "Vui lòng nhập vào xác nhận mật khẩu mới",
                requiredRepeatFalse: "Xác nhận mật khẩu không khớp với mật khẩu mới",
                passwordRegex: "Mật khẩu phải ít nhất 8 ký tự, chứa ít nhất 1 ký tự hoa, 1 ký tự thường, 1 chữ số và 1 ký tự đặc biệt"
            },
            rules: {
                requiredOldPassword: function (input) {
                    if (input.is("[name='oldPassword']") && input.val() == "") {
                        return false;
                    }
                    return true;
                },
                requiredNewPassword: function (input) {
                    if (input.is("[name='newPassword']") && input.val() == "") {
                        return false;
                    }
                    return true;
                },
                requiredRepeatPassword: function (input) {
                    if (input.is("[name='repeatPassword']") && input.val() == "") {
                        return false;
                    }
                    return true;
                },
                requiredRepeatFalse: function (input) {
                    if (input.is("[name='repeatPassword']") && input.val() != "") {
                        let newpass = $("[name='newPassword']").val();
                        if (newpass != input.val()) {
                            return false;
                        }
                    }
                    return true;
                },
                passwordRegex: function (input) {
                    if (input.is("[name='newPassword']")) {

                        var criteria = [
                            { regex: /.{8,}/, message: "Password must be at least 8 characters long." },
                            { regex: /[A-Z]/, message: "Password must contain at least one uppercase letter." },
                            { regex: /[a-z]/, message: "Password must contain at least one lowercase letter." },
                            { regex: /[0-9]/, message: "Password must contain at least one number." },
                            { regex: /[!@#$%^&*(),.?":{}|<>]/, message: "Password must contain at least one special character." }
                        ];
                        let newpass = $("[name='newPassword']").val();

                        var errors = criteria.filter(function (criterion) {
                            return !criterion.regex.test(newpass);
                        }).map(function (criterion) {
                            return criterion.message;
                        });

                        if (errors.length > 0) {
                            return false;
                        }
                    }
                    return true;
                }
            }
        });

        formChangePassword.bind("submit", function (e) {
            console.log(e.model);
            e.preventDefault();
            var validator = formPassword.data("kendoValidator");

            if (validator.validate()) {
                var response = ajax("POST", "/User/ChangePassword", e.model, () => {
                    if (kendoChangePasswordWindow != undefined)
                        kendoChangePasswordWindow.data("kendoWindow").close();
                })
            }
            else {
                return false;
            }
        });
        $('#oldPassword').on('keypress', function (event) {
            if (event.which === 32) { // 32 is the ASCII code for space
                event.preventDefault(); // Prevent the default action (space input)
            }
        });
        $('#newPassword').on('keypress', function (event) {
            if (event.which === 32) { // 32 is the ASCII code for space
                event.preventDefault(); // Prevent the default action (space input)
            }
        });
        $('#repeatPassword').on('keypress', function (event) {
            if (event.which === 32) { // 32 is the ASCII code for space
                event.preventDefault(); // Prevent the default action (space input)
            }
        });
    }

    function initChangPasswordKendoWindow (title, width, height) {
        kendoChangePasswordWindow = $("#ChangePassword").kendoWindow({
            width: width,
            title: title,
            height: height,
            resizable: false,
            visible: false,
            modal: true,
            actions: [
                "Close"
            ],
            scrollable: true
        });
    };

    $(".change-password").click(function () {
        initFormChangePassword();
        openChangPasswordKendoWindow();
    });
});
