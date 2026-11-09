/**
 * ReCar Real-time Notification System
 * Đồng bộ chuông thông báo giữa Khách hàng, Chủ xe và Quản trị viên
 */

(function () {
 window.RecarNotificationManager = {
 config: {
 bellBtnId: 'btnOwnerBell',
 dotId: 'ownerNotificationDot',
 badgeId: 'ownerNotificationBadge',
 listId: 'ownerNotificationList',
 emptyId: 'ownerNotificationEmpty',
 markAllBtnId: 'btnMarkAllRead'
 },

 init: function (options) {
 this.config = Object.assign({}, this.config, options);
 var self = this;

 // Bắt sự kiện click nút Đánh dấu tất cả đã đọc
 $(document).on('click', '#' + self.config.markAllBtnId, function (e) {
 e.preventDefault();
 e.stopPropagation();
 self.markAllAsRead();
 });

 // Khi click vào chuông thì tải thông báo mới nhất
 $(document).on('click', '#' + self.config.bellBtnId, function () {
 self.loadNotifications();
 });

 // Lắng nghe sự kiện click vào từng thông báo
 $(document).on('click', '.recar-notif-item', function (e) {
 var notifId = $(this).data('id');
 var linkUrl = $(this).data('url');
 var isRead = $(this).data('read');

 if (!isRead) {
 self.markAsRead(notifId);
 }

 if (linkUrl && linkUrl !== '#' && linkUrl !== 'javascript:void(0)') {
 window.location.href = linkUrl;
 }
 });

 // Tải thông báo lần đầu khi trang sẵn sàng
 self.loadNotifications();

 // Đăng ký nhận thông báo Real-time nếu SignalR connection đã kết nối
 self.setupSignalRListener();
 },

 loadNotifications: function () {
 var self = this;
 $.ajax({
 url: '/Notification/GetNotifications',
 type: 'GET',
 dataType: 'json',
 success: function (res) {
 if (res && res.success) {
 self.renderNotifications(res.notifications, res.unreadCount);
 }
 },
 error: function (err) {
 console.warn('Lỗi tải thông báo ReCar:', err);
 }
 });
 },

 renderNotifications: function (list, unreadCount) {
 var self = this;
 var listEl = $('#' + self.config.listId);
 var emptyEl = $('#' + self.config.emptyId);
 var dotEl = $('#' + self.config.dotId);
 var badgeEl = $('#' + self.config.badgeId);

 // Cập nhật số lượng thông báo chưa đọc
 if (unreadCount > 0) {
 dotEl.show();
 if (badgeEl.length) {
 badgeEl.text(unreadCount > 99 ? '99+' : unreadCount).show();
 }
 } else {
 dotEl.hide();
 if (badgeEl.length) badgeEl.hide();
 }

 if (!list || list.length === 0) {
 emptyEl.show();
 listEl.empty();
 return;
 }

 emptyEl.hide();
 var html = '';

 list.forEach(function (n) {
 var iconMeta = self.getIconMeta(n.type);
 var unreadClass = n.isRead ? '' : 'bg-light-subtle fw-semibold';
 var unreadIndicator = n.isRead ? '' : '<span class="position-absolute end-0 top-50 translate-middle-y me-3 p-1 bg-primary border border-light rounded-circle" style="width: 7px; height: 7px;"></span>';

 html += `
 <li class="recar-notif-item position-relative px-3 py-2.5 border-bottom text-dark text-decoration-none d-flex align-items-start gap-2.5 ${unreadClass}"
 data-id="${n.notificationId}" data-url="${n.linkUrl}" data-read="${n.isRead}"
 style="cursor: pointer; transition: background 0.15s ease; border-radius: 8px; margin: 2px 4px;">
 <div class="rounded-circle d-flex align-items-center justify-content-center flex-shrink-0" 
 style="width: 36px; height: 36px; background: ${iconMeta.bg}; color: ${iconMeta.color};">
 <i class="bi ${iconMeta.icon} fs-6"></i>
 </div>
 <div class="flex-grow-1 pe-2" style="min-width: 0;">
 <div class="d-flex align-items-center justify-content-between gap-1 mb-0.5">
 <span class="small fw-bold text-dark text-truncate d-block" style="font-size: 0.83rem;">${self.escapeHtml(n.title)}</span>
 <span class="text-muted flex-shrink-0" style="font-size: 0.72rem;">${n.timeAgo}</span>
 </div>
 <p class="text-secondary small mb-0 text-break" style="font-size: 0.77rem; line-height: 1.35; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden;">
 ${self.escapeHtml(n.content || '')}
 </p>
 </div>
 ${unreadIndicator}
 </li>`;
 });

 listEl.html(html);
 },

 getIconMeta: function (type) {
 switch (type) {
 case 'CarApproved':
 return { icon: 'bi-check-circle-fill', bg: '#dcfce7', color: '#16a34a' }; // Xanh lá
 case 'CarRejected':
 return { icon: 'bi-x-circle-fill', bg: '#fee2e2', color: '#dc2626' }; // Đỏ
 case 'CarPending':
 return { icon: 'bi-hourglass-split', bg: '#fef3c7', color: '#d97706' }; // Vàng cam
 case 'NewOrder':
 return { icon: 'bi-receipt-cutoff', bg: '#e0f2fe', color: '#0284c7' }; // Xanh ReCar
 case 'System':
 default:
 return { icon: 'bi-bell-fill', bg: '#f3e8ff', color: '#9333ea' }; // Tím
 }
 },

 markAsRead: function (id) {
 var self = this;
 $.post('/Notification/MarkRead', { id: id }, function () {
 self.loadNotifications();
 });
 },

 markAllAsRead: function () {
 var self = this;
 $.post('/Notification/MarkAllRead', {}, function () {
 self.loadNotifications();
 if (typeof Swal !== 'undefined') {
 const Toast = Swal.mixin({
 toast: true,
 position: 'top-end',
 showConfirmButton: false,
 timer: 1500,
 timerProgressBar: true
 });
 Toast.fire({
 icon: 'success',
 title: 'Đã đánh dấu đã đọc tất cả thông báo'
 });
 }
 });
 },

 setupSignalRListener: function () {
 var self = this;
 function attachHubListener(conn) {
 if (!conn) return;

 conn.on('ReceiveNotification', function (payload) {
 self.loadNotifications();
 self.showToastNotification(payload.title, payload.content, payload.type);
 });

 conn.on('ReceiveCarModerationResult', function (data) {
 self.loadNotifications();
 var title = data.isApproved ? 'Hồ sơ xe đã được phê duyệt ' : 'Hồ sơ xe bị từ chối duyệt ';
 var content = data.isApproved
 ? `Xe ${data.carName} (${data.licensePlate}) đã sẵn sàng cho thuê.`
 : `Lý do từ chối: ${data.reason}`;
 self.showToastNotification(title, content, data.isApproved ? 'CarApproved' : 'CarRejected');
 });

 conn.on('ReceiveNewCarPendingNotification', function (data) {
 self.loadNotifications();
 self.showToastNotification('Có xe mới chờ duyệt ', `Xe ${data.carName} (${data.licensePlate}) đang chờ phê duyệt.`, 'CarPending');
 });

 conn.on('ReceiveNewOrderNotification', function (data) {
 self.loadNotifications();
 self.showToastNotification('Có đơn đặt xe mới ', `Xe ${data.carName} - Đặt cọc: ${data.depositFormatted}`, 'NewOrder');
 });
 }

 // Thử bắt connection từ các biến SignalR phổ biến trong app
 if (window.recarHubConnection) {
 attachHubListener(window.recarHubConnection);
 } else if (window.connection) {
 attachHubListener(window.connection);
 } else {
 // Polling kiểm tra connection sẵn sàng
 var attempts = 0;
 var interval = setInterval(function () {
 attempts++;
 var conn = window.recarHubConnection || window.connection;
 if (conn) {
 clearInterval(interval);
 attachHubListener(conn);
 }
 if (attempts > 15) clearInterval(interval);
 }, 1000);
 }
 },

 showToastNotification: function (title, content, type) {
 if (typeof Swal !== 'undefined') {
 var meta = this.getIconMeta(type);
 const Toast = Swal.mixin({
 toast: true,
 position: 'top-end',
 showConfirmButton: false,
 timer: 4500,
 timerProgressBar: true
 });
 Toast.fire({
 iconHtml: `<i class="bi ${meta.icon}" style="color: ${meta.color}; font-size: 1.25rem;"></i>`,
 customClass: { icon: 'border-0' },
 title: `<div class="fw-bold" style="font-size: 0.85rem;">${this.escapeHtml(title)}</div>`,
 html: `<div class="text-secondary small" style="font-size: 0.78rem;">${this.escapeHtml(content || '')}</div>`
 });
 }
 },

 escapeHtml: function (string) {
 var entityMap = {
 '&': '&amp;',
 '<': '&lt;',
 '>': '&gt;',
 '"': '&quot;',
 "'": '&#39;',
 '/': '&#x2F;'
 };
 return String(string).replace(/[&<>"'\/]/g, function (s) {
 return entityMap[s];
 });
 }
 };
})();

