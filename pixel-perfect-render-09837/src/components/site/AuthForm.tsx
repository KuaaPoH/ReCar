import { Link } from "@tanstack/react-router";
import { useState } from "react";
import { Logo } from "./Layout";
import { images } from "@/lib/data";

const inp = "w-full rounded-lg border px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-ring";

export function AuthForm({ mode }: { mode: "login" | "register" }) {
  const [msg, setMsg] = useState("");
  const reg = mode === "register";
  return (
    <div className="grid min-h-screen lg:grid-cols-2">
      <div className="flex items-center justify-center p-6">
        <form className="w-full max-w-sm space-y-4" onSubmit={(e) => {
          e.preventDefault();
          const fd = new FormData(e.currentTarget);
          if (reg && fd.get("p") !== fd.get("p2")) return setMsg("Mật khẩu nhập lại không khớp.");
          setMsg("Bản mẫu: chưa kết nối hệ thống đăng nhập.");
        }}>
          <Logo />
          <h1 className="pt-4 text-2xl font-bold text-navy">{reg ? "Tạo tài khoản ReCar" : "Chào mừng trở lại"}</h1>
          {reg && <input required name="n" className={inp} placeholder="Họ và tên" />}
          <input required type="email" className={inp} placeholder="Email" />
          {reg && <input required className={inp} placeholder="Số điện thoại" pattern="0[0-9]{9}" />}
          <input required name="p" type="password" minLength={6} className={inp} placeholder="Mật khẩu" />
          {reg && <input required name="p2" type="password" className={inp} placeholder="Nhập lại mật khẩu" />}
          {!reg && <div className="flex justify-between text-sm"><label className="flex gap-2"><input type="checkbox" />Ghi nhớ</label><a className="text-primary">Quên mật khẩu?</a></div>}
          {msg && <p className="text-sm text-warning">{msg}</p>}
          <button className="w-full rounded-lg bg-primary py-3 font-semibold text-primary-foreground">{reg ? "Đăng ký" : "Đăng nhập"}</button>
          <p className="text-center text-sm text-muted-foreground">
            {reg ? <>Đã có tài khoản? <Link to="/login" className="text-primary">Đăng nhập</Link></> : <>Chưa có tài khoản? <Link to="/register" className="text-primary">Đăng ký</Link></>}
          </p>
        </form>
      </div>
      <div className="relative hidden lg:block">
        <img src={images.hero} alt="" className="absolute inset-0 h-full w-full object-cover" />
        <div className="absolute inset-0 bg-navy/50" />
        <p className="absolute bottom-12 left-12 max-w-md text-3xl font-bold text-navy-foreground">Chuyến đi của bạn, lựa chọn của bạn.</p>
      </div>
    </div>
  );
}
