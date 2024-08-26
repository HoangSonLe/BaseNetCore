function CheckPermission() {
    let permission = "._permission_";
    setTimeout(() => {
        $(permission).toArray().forEach(e => {
            let _enum = e?.dataset?.enum;
            if (_enum) {
                let findPermission = Userdata.enumActionList.find(i => i == _enum);
                if (!findPermission) e.remove();

            }
        })
    },200);
}

function checkPermissionGroupExist(list, listPermission) { //["1","2"],["1","2","3","4"]
    let rs = false;
    if (list != null && list.length > 0) {
        for (let i = 0; i < list.length; i++) {
            let el = list[i];
            if (listPermission.find(e => e == el)) {
                rs = true;
                break;
            }
        }
    }
    return rs;
}
