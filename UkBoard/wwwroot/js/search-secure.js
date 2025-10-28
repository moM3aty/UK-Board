document.addEventListener('DOMContentLoaded', function () {
    'use strict';

    const input = document.getElementById("searchInput");
    const display = document.getElementById("certificateDisplay");
    const loadingSpinner = document.getElementById("loadingSpinner");
    let fetchTimeout;

    input.addEventListener("input", () => {
        // إلغاء أي طلب بحث معلق
        if (fetchTimeout) {
            clearTimeout(fetchTimeout);
        }

        const value = input.value.trim();

        if (value === "") {
            display.innerHTML = ""; // مسح النتائج إذا كان الحقل فارغاً
            loadingSpinner.style.display = 'none';
            return;
        }

        // إظهار علامة التحميل
        loadingSpinner.style.display = 'block';
        display.innerHTML = ''; // مسح النتائج القديمة

        // تأخير طلب البحث قليلاً (debounce)
        fetchTimeout = setTimeout(() => {
            fetch(`/Home/SearchCertificate?id=${encodeURIComponent(value)}`)
                .then(response => response.json())
                .then(data => {
                    loadingSpinner.style.display = 'none'; // إخفاء التحميل
                    if (data.success) {
                        // إنشاء الحاوية الآمنة وتطبيق الحماية
                        display.innerHTML = `
                            <div id="secure-image-container" class="secure-container" oncontextmenu="return false;">
                                <img src="/${data.imagePath}" alt="Certificate" class="img-fluid rounded shadow secure-image">
                            </div>
                        `;
                    } else {
                        display.innerHTML = `
                            <p class="text-danger text-center mt-2">${data.message}</p>
                        `;
                    }
                })
                .catch(error => {
                    loadingSpinner.style.display = 'none';
                    display.innerHTML = `
                        <p class="text-danger text-center mt-2">An error occurred while searching.</p>
                    `;
                });
        }, 500); // تأخير 500ms
    });

    // === طبقة الحماية ===

    // 1. منع كليك يمين (تمت إضافته أيضاً في HTML لضمان الفعالية)
    document.addEventListener('contextmenu', function (e) {
        if (document.getElementById('secure-image-container')) {
            e.preventDefault();
        }
    });

    // 2. منع اختصارات لوحة المفاتيح
    document.addEventListener('keydown', function (e) {
        // منع F12
        if (e.key === 'F12') {
            e.preventDefault();
        }

        // منع PrintScreen (قد لا تعمل في كل المتصفحات)
        if (e.key === 'PrintScreen') {
            e.preventDefault();
            // محاولة إظهار رسالة للمستخدم
            alert("Screenshots are disabled for this page.");
        }

        // منع Ctrl+C, Ctrl+P, Ctrl+U, Ctrl+S
        if (e.ctrlKey && (e.key === 'c' || e.key === 'p' || e.key === 'u' || e.key === 's')) {
            e.preventDefault();
        }

        // منع Command+C, Command+P (لأجهزة Mac)
        if (e.metaKey && (e.key === 'c' || e.key === 'p')) {
            e.preventDefault();
        }
    });

    // 3. منع لقطة الشاشة (تجريبي باستخدام CSS - انظر secure-certificate.css)
    // لا يوجد حل JavaScript مباشر لمنع لقطات الشاشة، نعتمد على CSS و منع الاختصارات
});

