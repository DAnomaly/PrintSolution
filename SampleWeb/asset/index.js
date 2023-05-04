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
            document.getElementById('alertMessage').style.color = '#D21312';
        }
    });
}

function generateSelectOptions() {
    const printerListElement = document.getElementById('printerList');
    printerListElement.innerHTML = '';
    
    const preparedGroup = document.createElement('optgroup');
    const unpreparedGroup = document.createElement('optgroup');
    const noneGroup = document.createElement('optgroup');

    preparedGroup.label = 'Prepared Printers';
    unpreparedGroup.label = 'Unprepared Printers';
    noneGroup.label = 'Unkown Printers';

    for (let i = 0; i < data.length; i++) {
        var optionTag = document.createElement('option');
        optionTag.value = i;
        optionTag.innerText = data[i].name;

        if (data[i].useYN == true) {
            if (data[i].status.toLowerCase() != 'none') {
                preparedGroup.appendChild(optionTag);
            } else {
                noneGroup.appendChild(optionTag);
            }
        } else {
            unpreparedGroup.appendChild(optionTag);
        }
    }

    if (preparedGroup.childNodes.length > 0) {
        printerListElement.appendChild(preparedGroup);
    }
    if (noneGroup.childNodes.length > 0) {
        printerListElement.appendChild(noneGroup);
    }
    if (unpreparedGroup.childNodes.length > 0) {
        printerListElement.appendChild(unpreparedGroup);
    }

    let nowIndex = printerListElement.value;
    loadPrinterStatus(nowIndex);
    checkUseYn(nowIndex);
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

    var selectValue = document.getElementById('printerList').value;
    let printerName = data[Number.parseInt(selectValue)].name;
    // console.log('printerName', printerName);

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

                setTimeout(function() {
                    orderPrint(result.filename, printerName);
                }, 500);
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
    const submitMessage = document.getElementById('useYn');
    submitMessage.innerText = '프린트 요청시작..';
    submitMessage.style.color = '#131313';

    $.ajax({
        url: 'http://localhost:9203/Print/Call',
        type: 'GET',
        data: {"printer":printer, "filename":filename},
        dataType: 'JSON',
        success: function(response) {
            if (response.result == true) {
                submitMessage.innerText = '프린트 요청성공';
                submitMessage.style.color = '#131313';
            } else {
                submitMessage.innerText = '요청실패 : ' + response.message;
                submitMessage.style.color = '#D21312';
            }
        },
        error: function() {
            submitMessage.innerText = '실패 : 알수없는 원인으로 프린트 요청에 실패하였습니다. 관리자에게 문의하세요.';
            submitMessage.style.color = '#D21312';
        }
    });
}