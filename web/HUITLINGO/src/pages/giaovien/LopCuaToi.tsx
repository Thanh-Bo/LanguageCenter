import { useEffect, useState } from "react";
import { Icon } from "../../components/BieuTuong";
import { goiAPI, type ClassItem, type StudentItem } from "../../lib/goiAPI";
import { dungXacThuc } from "../../contexts/NguCanhXacThuc";

export default function LopCuaToi() {
  const { nguoiDung } = dungXacThuc();
  const [dsLop, setDsLop] = useState<ClassItem[]>([]);
  const [dangTai, setDangTai] = useState(true);
  const [lopChon, setLopChon] = useState<ClassItem | null>(null);
  const [dsHocVien, setDsHocVien] = useState<StudentItem[]>([]);
  const [dangTaiHV, setDangTaiHV] = useState(false);
  const [boLoc, setBoLoc] = useState<
    "Tất cả" | "Đang học" | "Sắp mở" | "Sắp đầy" | "Đã kết thúc"
  >("Tất cả");

  // Load danh sách lớp khi vào trang
  useEffect(() => {
    if (!nguoiDung) return;
    setDangTai(true);
    goiAPI.layLopCuaToi(nguoiDung.id).then((data) => {
      setDsLop(data);
      setDangTai(false);
    });
  }, [nguoiDung]);

  // Load học viên khi chọn lớp
  useEffect(() => {
    if (!lopChon) return;
    setDangTaiHV(true);
    goiAPI.layHocVienTheoLop(lopChon.maLop).then((data) => {
      setDsHocVien(data);
      setDangTaiHV(false);
    });
  }, [lopChon]);

  const dsLoc =
    boLoc === "Tất cả" ? dsLop : dsLop.filter((l) => l.trangThai === boLoc);

  return (
    <div className="p-4 sm:p-7">
      {/* Header */}
      <div className="mb-6 flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
        <div>
          <p className="mb-1 text-sm font-semibold text-blue-600">Giảng dạy</p>
          <h1 className="text-3xl font-bold text-slate-900">Lớp của tôi</h1>
          <p className="mt-2 text-sm text-slate-500">
            {dsLop.length} lớp đang giảng dạy · Theo dõi sĩ số và tiến độ học
            viên.
          </p>
        </div>
      </div>

      {/* Bộ lọc */}
      <div className="mb-6 flex flex-wrap gap-2">
        {(
          ["Tất cả", "Đang học", "Sắp mở", "Sắp đầy", "Đã kết thúc"] as const
        ).map((f) => (
          <button
            key={f}
            onClick={() => setBoLoc(f)}
            className={`rounded-xl px-4 py-2 text-sm font-semibold transition ${
              boLoc === f
                ? "bg-blue-600 text-white shadow-sm shadow-blue-200"
                : "border border-slate-200 bg-white text-slate-600 hover:border-blue-200"
            }`}
          >
            {f}
          </button>
        ))}
      </div>

      {/* Danh sách lớp */}
      {dangTai ? (
        <div className="grid place-items-center py-20 text-slate-400">
          Đang tải danh sách lớp...
        </div>
      ) : dsLoc.length === 0 ? (
        <div className="grid place-items-center py-20 text-slate-400">
          Chưa có lớp nào
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-5 md:grid-cols-2 xl:grid-cols-3">
          {dsLoc.map((lop) => (
            <button
              key={lop.id}
              onClick={() => setLopChon(lop)}
              className="group rounded-2xl border border-slate-200 bg-white p-5 text-left shadow-sm transition hover:-translate-y-0.5 hover:border-blue-200 hover:shadow-md"
            >
              <div className="flex items-start justify-between gap-3">
                <div>
                  <h3 className="text-lg font-bold text-slate-900">
                    {lop.tenLop}
                  </h3>
                  <span className="mt-1 inline-block rounded-full bg-blue-50 px-2.5 py-0.5 text-xs font-semibold text-blue-700">
                    {lop.maLop}
                  </span>
                </div>
                <span
                  className={`rounded-full px-2.5 py-1 text-xs font-bold ${
                    lop.trangThai === "Sắp đầy"
                      ? "bg-amber-100 text-amber-700"
                      : lop.trangThai === "Sắp mở"
                        ? "bg-blue-100 text-blue-700"
                        : lop.trangThai === "Đã kết thúc"
                          ? "bg-slate-200 text-slate-600"
                          : "bg-blue-600 text-white" // Đang học
                  }`}
                >
                  {lop.trangThai}
                </span>
              </div>

              <div className="mt-4 space-y-2 text-sm text-slate-600">
                <div className="flex items-center gap-2">
                  <Icon name="users" className="size-4 text-slate-400" />
                  {lop.siSo}/{lop.siSoToiDa} học viên
                </div>
                <div className="flex items-center gap-2">
                  <Icon name="clock" className="size-4 text-slate-400" />
                  {lop.lichHoc}
                </div>
                <div className="flex items-center gap-2">
                  <Icon name="room" className="size-4 text-slate-400" />
                  Phòng {lop.phong} · {lop.capDo}
                </div>
              </div>

              <div className="mt-5">
                <div className="mb-1 flex justify-between text-xs text-slate-500">
                  <span>Tiến độ khóa học</span>
                  <span className="font-semibold text-blue-600">
                    {lop.tienDo}%
                  </span>
                </div>
                <div className="h-2 w-full rounded-full bg-blue-50">
                  <div
                    className="h-2 rounded-full bg-blue-600"
                    style={{ width: `${lop.tienDo}%` }}
                  />
                </div>
              </div>

              <div className="mt-4 flex items-center justify-between text-xs text-slate-400">
                <span>
                  {lop.ngayBatDau} → {lop.ngayKetThuc}
                </span>
                <span className="font-semibold text-blue-600 opacity-0 transition group-hover:opacity-100">
                  Xem chi tiết →
                </span>
              </div>
            </button>
          ))}
        </div>
      )}

      {/* Modal chi tiết lớp */}
      {lopChon && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/50 p-4 backdrop-blur-sm"
          onClick={() => setLopChon(null)}
        >
          <div
            className="max-h-[90vh] w-full max-w-3xl overflow-y-auto rounded-2xl bg-white shadow-2xl"
            onClick={(e) => e.stopPropagation()}
          >
            {/* Header modal */}
            <div className="sticky top-0 z-10 flex items-start justify-between gap-4 border-b border-slate-100 bg-white p-6">
              <div>
                <h2 className="text-2xl font-bold text-slate-900">
                  {lopChon.tenLop}
                </h2>
                <p className="mt-1 text-sm text-slate-500">
                  {lopChon.maLop} · Phòng {lopChon.phong} · {lopChon.lichHoc}
                </p>
              </div>
              <button
                onClick={() => setLopChon(null)}
                className="grid size-9 place-items-center rounded-full text-slate-400 hover:bg-slate-100 hover:text-slate-600"
              >
                <Icon name="close" className="size-5" />
              </button>
            </div>

            {/* Stats */}
            <div className="grid grid-cols-3 gap-3 border-b border-slate-100 p-6">
              <div className="rounded-xl bg-blue-50 p-3 text-center">
                <div className="text-xs font-semibold uppercase text-slate-500">
                  Sĩ số
                </div>
                <div className="mt-1 text-2xl font-bold text-blue-600">
                  {lopChon.siSo}/{lopChon.siSoToiDa}
                </div>
              </div>
              <div className="rounded-xl bg-blue-50 p-3 text-center">
                <div className="text-xs font-semibold uppercase text-slate-500">
                  Tiến độ
                </div>
                <div className="mt-1 text-2xl font-bold text-blue-600">
                  {lopChon.tienDo}%
                </div>
              </div>
              <div className="rounded-xl bg-blue-50 p-3 text-center">
                <div className="text-xs font-semibold uppercase text-slate-500">
                  Cấp độ
                </div>
                <div className="mt-1 text-lg font-bold text-blue-600">
                  {lopChon.capDo}
                </div>
              </div>
            </div>

            {/* Danh sách học viên */}
            <div className="p-6">
              <h3 className="mb-4 text-lg font-bold text-slate-900">
                Danh sách học viên ({dsHocVien.length})
              </h3>

              {dangTaiHV ? (
                <div className="py-8 text-center text-slate-400">
                  Đang tải danh sách học viên...
                </div>
              ) : dsHocVien.length === 0 ? (
                <div className="py-8 text-center text-slate-400">
                  Chưa có học viên nào
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full border-collapse text-left text-sm">
                    <thead>
                      <tr className="border-b border-slate-200 bg-slate-50 text-xs uppercase tracking-wider text-slate-500">
                        <th className="px-3 py-3 font-bold">Mã HV</th>
                        <th className="px-3 py-3 font-bold">Họ tên</th>
                        <th className="px-3 py-3 font-bold">Email</th>
                        <th className="px-3 py-3 font-bold">SĐT</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100">
                      {dsHocVien.map((hv) => (
                        <tr key={hv.id} className="hover:bg-slate-50">
                          <td className="px-3 py-3 font-semibold text-blue-600">
                            {hv.maHV}
                          </td>
                          <td className="px-3 py-3 font-medium text-slate-900">
                            {hv.hoTen}
                          </td>
                          <td className="px-3 py-3 text-slate-500">
                            {hv.email}
                          </td>
                          <td className="px-3 py-3 text-slate-500">{hv.sdt}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
