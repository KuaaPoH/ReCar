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
  pricingList: "GET /CarOwner/Pricing/Index",
  pricingSave: "POST /CarOwner/Pricing/Save (rules; owner inferred from session)",
  pricingPreview:
    "POST /CarOwner/Pricing/Preview (CarId, PickupAt, ReturnAt; server calculates final price)",
  calendarList: "GET /CarOwner/Calendar/Index?start=&end=&carId=",
  calendarBlock: "POST /CarOwner/Calendar/Block (recheck ownership and conflicts atomically)",
  calendarUnblock: "POST /CarOwner/Calendar/Unblock/{id} (antiforgery token)",
  analytics: "GET /CarOwner/Analytics/Index?start=&end=&carId=&brand=&segment=",
  wallet: "GET /CarOwner/Wallet/Index?start=&end=&status=",
  payoutDetails: "GET /CarOwner/Wallet/PayoutDetails/{id}",
  exportExcel: "GET /CarOwner/Wallet/ExportExcel?start=&end=",
  exportPdf: "GET /CarOwner/Wallet/ExportPdf?start=&end=",
  signalRHub: "/hubs/carowner-notifications",
  logout: "POST /Account/Logout (antiforgery token)",
} as const;

export const DEMO_NOTE = "Mô phỏng giao diện — chưa lưu vào hệ thống.";
