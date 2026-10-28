// Điểm tích hợp ASP.NET Core MVC — Area "CarOwner".
// Prototype KHÔNG gọi các endpoint này; mọi thao tác hiện chỉ là mô phỏng phía trình duyệt.
// Backend phải xác định OwnerId từ tài khoản đăng nhập (không tin OwnerId từ frontend)
// và kiểm tra lại mọi quy tắc trong rules.ts.
export const API = {
  dashboard: "GET /CarOwner/Home/Index",
  carsList: "GET /CarOwner/Cars/Index",
  carCreate: "POST /CarOwner/Cars/Create (multipart/form-data)",
  carEdit: (id: number) => `POST /CarOwner/Cars/Edit/${id}`,
  carDelete: (id: number) => `POST /CarOwner/Cars/Delete/${id}`,
  toggleStatus: (id: number) => `POST /CarOwner/Cars/ToggleStatus/${id}`,
  toggleSale: (id: number) => `POST /CarOwner/Cars/ToggleSale/${id}`,
  ordersList: "GET /CarOwner/Orders/Index",
  handover: (id: string) => `POST /CarOwner/Orders/Handover/${id}`,
  receiveBack: (id: string) => `POST /CarOwner/Orders/Complete/${id}`,
  cancel: (id: string) => `POST /CarOwner/Orders/Cancel/${id}`,
  registerUpdate: "POST /CarOwner/Register/Update",
  signalRHub: "/hubs/carowner-notifications",
  logout: "POST /Account/Logout (antiforgery token)",
} as const;

export const DEMO_NOTE = "Mô phỏng giao diện — chưa lưu vào hệ thống.";
