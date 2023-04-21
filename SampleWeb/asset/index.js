document.addEventListener("DOMContentLoaded", function(){
    document.getElementById('printerList').addEventListener('change', printerListChangeEvent);
});

const httpRequest = new XMLHttpRequest();
httpRequest.onreadystatechange = handler;
httpRequest.open("GET", "http://localhost:9203/print/show", true);
httpRequest.send();

let data;
function handler() {
    if (httpRequest.readyState === XMLHttpRequest.DONE) {
        if (httpRequest.status === 200) {
            data = JSON.parse(httpRequest.responseText);
            console.log(data);

            generateSelectOptions();

        } else {
            document.getElementById('alertMessage').innerHTML 
                = "서버로부터 응답을 받을 수 없습니다. 서버가 동작중인지 확인해 주세요.";
        }
    }
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
    } else {
        document.getElementById('useYn').innerText = '사용불가능';
    }
    
}

function printerListChangeEvent(e) {
    var index = e.target.value;

    loadPrinterStatus(index);
    checkUseYn(0);
}