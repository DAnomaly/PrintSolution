document.addEventListener('DOMContentLoaded', function(){
    onLoadEvent();

    document.getElementById('printerList').addEventListener('change', printerListChangeEvent);
    document.getElementById('submitButton').addEventListener('click', submitButtonClickEvent);
});

let data;
function onLoadEvent() {
    $.ajax({
        url: 'http://localhost:9203/Print/Show',
        type: 'GET',
        dataType: 'JSON',
        success: function (result) {
            data = result;
            generateSelectOptions();
        },
        error: function () {
            document.getElementById('alertMessage').innerHTML 
                = '서버로부터 응답을 받을 수 없습니다. 관리자에게 문의하세요.';
        }
    });
}

function generateSelectOptions() {
    document.getElementById('printerList').innerHTML = '';
    for (let i = 0; i < data.length; i++) {
        var optionTag = document.createElement('option');
        optionTag.value = i;
        optionTag.innerText = data[i].name;

        document.getElementById('printerList').appendChild(optionTag);
    }

    loadPrinterStatus(0);
    checkUseYn(0);
}

function loadPrinterStatus(index) {
    var status = data[index].status;
    if (status.indexOf('||') != -1) {
        status = status.substring(status.lastIndexOf('||') + 2).trim();
    }
    document.getElementById('printerStatus').innerText = status;
}

function checkUseYn(index) {
    var status = data[index].status;
    if (status.indexOf('||') != -1) {
        status = status.substring(0, status.lastIndexOf('||')).trim().toLowerCase();
    } else {
        status = data[index].useYN;
    }
    
    if (status == 'true' || status == true) {
        document.getElementById('useYn').innerText = '사용가능';
        document.getElementById('useYn').style.color = '#121212';
    } else {
        document.getElementById('useYn').innerText = '사용불가능';
        document.getElementById('useYn').style.color = '#D21312';
    }
    
}

function printerListChangeEvent(e) {
    var index = e.target.value;

    loadPrinterStatus(index);
    checkUseYn(0);
}

function submitButtonClickEvent(e) {

    const submitMessage = document.getElementById('useYn');
    submitMessage.innerText = '파일 전송중...';
    submitMessage.style.color = '#131313';

    var formElement = document.createElement('form');
    var inputElement = document.createElement('input');
    inputElement.name = 'file';
    inputElement.type = 'file';
    inputElement.files = document.getElementById('fileSelect').files;
    formElement.appendChild(inputElement);
    var formData = new FormData(formElement);

    let printerName = "";
    if (printerName == "") {
        var printerList = document.getElementById('printerList');
        var selectValue = printerList.value;
        var options = printerList.childNodes;
        for (let i = 0; i < options.length; i++) {
            if (selectValue == options[i].value) {
                printerName = options.innerText;
                break;
            }
        }
    }

    console.log('DEBUG: File Send Start.')
    $.ajax({
        url: 'http://localhost:9203/Print/Upload',
        enctype: 'multipart/form-data',
        type: 'POST',
        data: formData,
        dataType: 'JSON',
        contentType : false,
        processData : false,
        success: function(result) {
            if (result.message == 'Success') {
                submitMessage.innerText = '파일 전송완료';
                submitMessage.style.color = '#131313';

                setTimeout(orderPrint, 500);
            } else {
                submitMessage.innerText = '실패 : ' 
                    + result.message.substring(result.message.indexOf(':') + 2);
            }
        },
        error:  function() {
            submitMessage.innerText = '실패 : 잘못된 요청입니다. 관리자에게 문의하세요.';
            submitMessage.style.color = '#D21312';
        }
    });
}

function orderPrint(filename, printer) {
    // TODO
}