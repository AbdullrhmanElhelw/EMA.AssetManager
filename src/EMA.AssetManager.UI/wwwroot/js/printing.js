window.printDiv = function (divId) {
    var divElement = document.getElementById(divId);
    if (!divElement) {
        console.error("Element not found: " + divId);
        return;
    }

    var content = divElement.innerHTML;
    var myWindow = window.open('', '', 'width=800,height=600');

    if (!myWindow) {
        alert("يرجى السماح بالنوافذ المنبثقة (Pop-ups) للطباعة.");
        return;
    }

    myWindow.document.write('<html><head><title>طباعة تقرير - الأكاديمية العسكرية</title>');

    // إضافة خطوط وتنسيقات
    myWindow.document.write('<link href="https://fonts.googleapis.com/css2?family=Cairo:wght@400;700&display=swap" rel="stylesheet">');
    myWindow.document.write('<style>');
    myWindow.document.write('body { font-family: "Cairo", sans-serif; direction: rtl; padding: 20px; }');
    myWindow.document.write('table { width: 100%; border-collapse: collapse; margin-top: 20px; }');
    myWindow.document.write('th, td { border: 1px solid #000; padding: 8px; text-align: center; }');
    myWindow.document.write('th { background-color: #f0f0f0; }');
    myWindow.document.write('.no-print { display: none; }'); // إخفاء الأزرار في الطباعة
    myWindow.document.write('</style>');

    myWindow.document.write('</head><body>');
    myWindow.document.write(content);
    myWindow.document.write('</body></html>');

    myWindow.document.close();
    myWindow.focus();

    // مهلة صغيرة لتحميل المحتوى قبل الطباعة
    setTimeout(function () {
        myWindow.print();
        myWindow.close();
    }, 500);
}