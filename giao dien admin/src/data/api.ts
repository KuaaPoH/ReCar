/**
 * API DỰ KIẾN phía ASP.NET Core MVC (Areas/Admin). Prototype hiện dùng
 * src/data/store.ts (dữ liệu demo). Khi tích hợp, thay từng hàm mutate
 * bằng lời gọi tương ứng dưới đây. Phân quyền, xác minh thanh toán và ghi
 * audit log PHẢI thực thi ở backend.
 */
export const ADMIN_API = {
  dashboard: "GET  /Admin/Dashboard/Summary",
  pendingCars: "GET  /Admin/Cars/Pending",
  approveCar: "POST /Admin/Cars/{id}/Approve",
  rejectCar: "POST /Admin/Cars/{id}/Reject   { reason, notifyOwner }",
  cars: "GET  /Admin/Cars?keyword&brand&segment&status&city&ownerId&page",
  suspendCar: "POST /Admin/Cars/{id}/Suspend  { reason }",
  restoreCar: "POST /Admin/Cars/{id}/Restore",
  hideCar: "POST /Admin/Cars/{id}/Hide     { reason }",
  orders: "GET  /Admin/Orders?status&from&to&q&method&page",
  orderDetail: "GET  /Admin/Orders/{id}",
  cancelOrder: "POST /Admin/Orders/{id}/Cancel  { reason }",
  refundOrder: "POST /Admin/Orders/{id}/Refund  { reason, amount }",
  finance: "GET  /Admin/Finance/Summary",
  settlements: "GET  /Admin/Finance/Settlements?page",
  confirmPayout: "POST /Admin/Finance/Settlements/{id}/ConfirmTransfer { reference }",
  users: "GET  /Admin/Users?tab=customers|owners|admins",
  lockUser: "POST /Admin/Users/{id}/Lock   { reason }",
  unlockUser: "POST /Admin/Users/{id}/Unlock",
  resetPassword: "POST /Admin/Users/{id}/SendResetLink",
  posts: "GET  /Admin/Blog",
  savePost: "POST /Admin/Blog/Save { ..., status }",
  settings: "GET|POST /Admin/Settings",
} as const;
