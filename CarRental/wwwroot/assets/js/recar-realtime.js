/**
 * ReCar Real-time System (SignalR Client)
 * Tích hợp công nghệ WebSockets thời gian thực cho Chat, Chuông thông báo đẩy & Quản lý xe
 */

(function () {
    // 1. Tạo hiệu ứng âm thanh thông báo bằng Web Audio API (chuẩn âm sắc du dương, 100% offline không cần file mp3 ngoài)
    var audioCtx = null;
    function playNotificationSound(type) {
        try {
            var AudioContext = window.AudioContext || window.webkitAudioContext;
            if (!AudioContext) return;
            if (!audioCtx) {
                audioCtx = new AudioContext();
            }
            if (audioCtx.state === 'suspended') {
                audioCtx.resume();
            }

            var now = audioCtx.currentTime;
            var osc = audioCtx.createOscillator();
            var gain = audioCtx.createGain();

            osc.type = 'sine';
            if (type === 'chat') {
                // Âm thanh nhẹ nhàng "pop/ping" cho tin nhắn chat
                osc.frequency.setValueAtTime(587.33, now); // D5
                osc.frequency.exponentialRampToValueAtTime(880, now + 0.12); // A5
                gain.gain.setValueAtTime(0.12, now);
                gain.gain.exponentialRampToValueAtTime(0.001, now + 0.25);
                osc.connect(gain);
                gain.connect(audioCtx.destination);
                osc.start(now);
                osc.stop(now + 0.25);
            } else {
                // Âm thanh 2 nốt "chime" sang trọng cho đơn hàng mới & thông báo quan trọng
                osc.frequency.setValueAtTime(523.25, now); // C5
                osc.frequency.setValueAtTime(659.25, now + 0.1); // E5
                osc.frequency.setValueAtTime(783.99, now + 0.2); // G5
                gain.gain.setValueAtTime(0.15, now);
                gain.gain.exponentialRampToValueAtTime(0.001, now + 0.45);
                osc.connect(gain);
                gain.connect(audioCtx.destination);
                osc.start(now);
                osc.stop(now + 0.45);
            }
        } catch (e) {
            console.log("Audio not supported or blocked by browser policy:", e);
        }
    }

    // 2. Component Toast Notification SaaS Hiện Đại (Tự động tiêm container nếu chưa có)
    function ensureToastContainer() {
        var container = document.getElementById('recar-toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'recar-toast-container';
            container.style.cssText = 'position: fixed; top: 24px; right: 24px; z-index: 99999; display: flex; flex-direction: column; gap: 12px; max-width: 380px; width: 100%; pointer-events: none;';
            document.body.appendChild(container);
        }
        return container;
    }

    window.showRealtimeToast = function (options) {
        var container = ensureToastContainer();
        var toast = document.createElement('div');
        toast.style.cssText = 'pointer-events: auto; background: #ffffff; border-radius: 14px; box-shadow: 0 10px 25px -5px rgba(15, 23, 42, 0.15), 0 8px 10px -6px rgba(15, 23, 42, 0.1); border: 1px solid #e2e8f0; overflow: hidden; transform: translateX(120%); transition: transform 0.35s cubic-bezier(0.16, 1, 0.3, 1), opacity 0.35s ease; opacity: 0;';

        var iconHtml = '<i class="bi bi-bell-fill text-primary"></i>';
        var borderLeftColor = '#0284c7';
        if (options.type === 'order') {
            iconHtml = '<i class="bi bi-bag-check-fill" style="color: #10b981;"></i>';
            borderLeftColor = '#10b981';
        } else if (options.type === 'chat') {
            iconHtml = '<i class="bi bi-chat-dots-fill" style="color: #0284c7;"></i>';
            borderLeftColor = '#0284c7';
        } else if (options.type === 'car_pending') {
            iconHtml = '<i class="bi bi-shield-exclamation" style="color: #f59e0b;"></i>';
            borderLeftColor = '#f59e0b';
        } else if (options.type === 'approved') {
            iconHtml = '<i class="bi bi-check-circle-fill" style="color: #10b981;"></i>';
            borderLeftColor = '#10b981';
        } else if (options.type === 'rejected') {
            iconHtml = '<i class="bi bi-x-circle-fill" style="color: #ef4444;"></i>';
            borderLeftColor = '#ef4444';
        }

        toast.innerHTML = `
            <div style="border-left: 4px solid ${borderLeftColor}; padding: 14px 16px; display: flex; align-items: flex-start; gap: 12px;">
                <div style="font-size: 1.35rem; line-height: 1; flex-shrink: 0; margin-top: 2px;">
                    ${iconHtml}
                </div>
                <div style="flex-grow: 1; min-width: 0;">
                    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 3px;">
                        <span style="font-weight: 700; font-size: 0.9rem; color: #0f172a;">${options.title || 'Thông báo ReCar'}</span>
                        <span style="font-size: 0.72rem; color: #94a3b8; font-weight: 500;">Vừa xong</span>
                    </div>
                    <div style="font-size: 0.825rem; color: #475569; line-height: 1.4; word-break: break-word;">
                        ${options.message || ''}
                    </div>
                    ${options.actionText && options.actionUrl ? `
                        <div style="margin-top: 8px;">
                            <a href="${options.actionUrl}" style="display: inline-flex; align-items: center; gap: 4px; font-size: 0.8rem; font-weight: 600; color: #0284c7; text-decoration: none;">
                                ${options.actionText} <i class="bi bi-arrow-right" style="font-size: 0.75rem;"></i>
                            </a>
                        </div>
                    ` : ''}
                </div>
                <button type="button" class="btn-close" style="font-size: 0.7rem; flex-shrink: 0; opacity: 0.5;" aria-label="Close"></button>
            </div>
            <div style="height: 3px; background: ${borderLeftColor}; width: 100%; transform-origin: left; transition: transform 5.5s linear; transform: scaleX(1);" class="toast-progress"></div>
        `;

        container.appendChild(toast);

        // Hiển thị mượt mà
        requestAnimationFrame(function () {
            toast.style.transform = 'translateX(0)';
            toast.style.opacity = '1';
            var progress = toast.querySelector('.toast-progress');
            if (progress) {
                progress.style.transform = 'scaleX(0)';
            }
        });

        // Nút tắt
        var closeBtn = toast.querySelector('.btn-close');
        function removeToast() {
            toast.style.transform = 'translateX(120%)';
            toast.style.opacity = '0';
            setTimeout(function () {
                if (toast.parentNode) {
                    toast.parentNode.removeChild(toast);
                }
            }, 400);
        }

        if (closeBtn) {
            closeBtn.onclick = removeToast;
        }

        // Tự động tắt sau 6 giây
        var autoCloseTimeout = setTimeout(removeToast, 6000);
        toast.onmouseenter = function () {
            clearTimeout(autoCloseTimeout);
        };
        toast.onmouseleave = function () {
            autoCloseTimeout = setTimeout(removeToast, 2500);
        };
    };

    // 3. Khởi tạo SignalR Connection
    if (typeof signalR === 'undefined') {
        console.warn("SignalR library not found yet. Make sure vendor/signalr.min.js is included.");
        return;
    }

    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/recarHub")
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Information)
        .build();

    window.recarRealtimeHub = connection;

    function registerCurrentUser() {
        var user = window.RECAR_CURRENT_USER || {};
        var accountId = user.accountId || null;
        var role = user.role || null;
        var carId = user.currentCarId || null;

        connection.invoke("RegisterClient", accountId, role, carId)
            .then(function () {
                console.log("ReCar Realtime: Registered client successfully [ID: " + accountId + ", Role: " + role + "]");
            })
            .catch(function (err) {
                console.error("ReCar Realtime: RegisterClient failed:", err);
            });
    }

    // Kết nối đến Hub
    connection.start()
        .then(function () {
            console.log("ReCar Realtime Hub connected via WebSockets!");
            registerCurrentUser();
        })
        .catch(function (err) {
            console.error("ReCar Realtime Hub connection error:", err);
        });

    connection.onreconnected(function (connectionId) {
        console.log("ReCar Realtime Hub reconnected:", connectionId);
        registerCurrentUser();
    });

    // 4. LẮNG NGHE CÁC SỰ KIỆN THỜI GIAN THỰC

    // A. Tin nhắn Chat mới
    connection.on("ReceiveChatMessage", function (data) {
        playNotificationSound('chat');

        // Nếu giao diện Chat đang mở và đang xem đúng cuộc hội thoại
        if (typeof window.handleIncomingChatMessage === 'function') {
            var handled = window.handleIncomingChatMessage(data);
            if (handled) return;
        }

        // Nếu cửa sổ chat đang đóng hoặc ở trang khác -> hiện toast và nhảy badge tin nhắn
        showRealtimeToast({
            type: 'chat',
            title: data.senderName || 'Tin nhắn mới',
            message: data.messageText,
            actionText: 'Mở cửa sổ chat',
            actionUrl: 'javascript:void(0);'
        });

        // Nhảy badge chấm đỏ trên Header
        var chatDot = document.getElementById('chatNotificationDot');
        if (chatDot) {
            chatDot.style.display = 'block';
        }
    });

    // B. Chỉ báo đối phương đang gõ phím (Typing Indicator)
    connection.on("UserTyping", function (data) {
        if (typeof window.handleIncomingTyping === 'function') {
            window.handleIncomingTyping(data);
        }
    });

    // C. Đơn đặt xe mới (Dành cho Admin & Chủ xe)
    connection.on("ReceiveNewOrderNotification", function (data) {
        playNotificationSound('order');

        showRealtimeToast({
            type: 'order',
            title: '🎉 Đơn đặt xe mới #' + data.orderId,
            message: `Khách <strong>${data.customerName}</strong> vừa đặt xe <strong>${data.carName}</strong>. Đặt cọc: <strong>${data.depositFormatted}</strong>`,
            actionText: 'Xem chi tiết đơn',
            actionUrl: data.detailUrl || '/CarOwner/Orders/Index'
        });

        // Cập nhật chuông thông báo trên topbar nếu có
        var bellBadge = document.getElementById('adminNotificationBadge');
        if (bellBadge) {
            bellBadge.style.display = 'inline-block';
        }
    });

    // D. Xe mới đăng ký chờ phê duyệt (Dành cho Admin)
    connection.on("ReceiveNewCarPendingNotification", function (data) {
        playNotificationSound('alert');

        showRealtimeToast({
            type: 'car_pending',
            title: '🚗 Hồ sơ xe mới chờ duyệt',
            message: `Chủ xe <strong>${data.ownerName}</strong> vừa thêm xe <strong>${data.carName}</strong> (${data.licensePlate}).`,
            actionText: 'Đi tới duyệt xe',
            actionUrl: '/Admin/Cars/Pending'
        });

        // Tăng badge số lượng xe chờ duyệt trên Sidebar Admin
        var badge = document.getElementById('pendingCarsBadge');
        if (badge) {
            var currentCount = parseInt(badge.innerText.trim()) || 0;
            badge.innerText = currentCount + 1;
            badge.style.display = 'inline-block';
        }

        // Bật chấm đỏ trên chuông thông báo Admin
        var bellDot = document.getElementById('adminNotificationDot');
        if (bellDot) {
            bellDot.style.display = 'inline-block';
        }
    });

    // E. Kết quả phê duyệt xe (Dành cho Chủ xe)
    connection.on("ReceiveCarModerationResult", function (data) {
        if (data.isApproved) {
            playNotificationSound('order');
            showRealtimeToast({
                type: 'approved',
                title: '✅ Xe đã được phê duyệt!',
                message: `Xe <strong>${data.carName}</strong> (${data.licensePlate}) đã được Ban quản trị ReCar phê duyệt và xuất hiện trên sàn.`,
                actionText: 'Quản lý xe',
                actionUrl: '/CarOwner/Cars/Index'
            });
        } else {
            playNotificationSound('alert');
            showRealtimeToast({
                type: 'rejected',
                title: '⚠️ Hồ sơ xe bị từ chối',
                message: `Xe <strong>${data.carName}</strong> (${data.licensePlate}) không được duyệt. Lý do: ${data.reason}`,
                actionText: 'Chỉnh sửa hồ sơ',
                actionUrl: '/CarOwner/Cars/Edit/' + data.carId
            });
        }
    });

})();
