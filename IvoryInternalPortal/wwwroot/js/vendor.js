const apiurl =
    window.location.hostname === "localhost"
        ? "https://localhost:44385"
        : "https://192.168.1.106:4432";

let vendorTable;
let vendorModal;

let allVendors = [];
let filteredVendors = [];
let currentPage = 1;
let pageSize = 10;

// OPEN CREATE
window.openCreateModal = function () {
    document.getElementById("vendorForm").reset();
    document.getElementById("VendorId").value = 0;
    document.getElementById("modalTitle").innerText = "Add Vendor";

    resetFileDisplays();

    vendorModal = new bootstrap.Modal(
        document.getElementById("vendorModal")
    );
    vendorModal.show();
    setTimeout(initSignaturePad, 300);

};

function resetFileDisplays() {
    const agreementDisplay = document.getElementById("agreementFileDisplay");
    const signatureDisplay = document.getElementById("signatureFileDisplay");

    if (agreementDisplay) agreementDisplay.innerText = "No file chosen";
    if (signatureDisplay) signatureDisplay.innerText = "No file chosen";

    // Optional: clear actual file inputs too
    const agreementInput = document.querySelector("[name='agreementFile']");
    const signatureInput = document.querySelector("[name='signatureFile']");

    if (agreementInput) agreementInput.value = "";
    if (signatureInput) signatureInput.value = "";
}


// OPEN EDIT
window.openEditModal = function (emp) {

    document.getElementById("modalTitle").innerText = "Edit Vendor";

    document.querySelector("[name='VendorId']").value = emp.vendorId;
    document.querySelector("[name='CompanyName']").value = emp.companyName;
    document.querySelector("[name='CompanyAddress']").value = emp.companyAddress || "";
    document.querySelector("[name='CompanyEmail']").value = emp.companyEmail || "";
    document.querySelector("[name='CompanyPhone']").value = emp.companyPhone || "";
    document.querySelector("[name='PocPhone']").value = emp.pocPhone || "";
    document.querySelector("[name='GstNumber']").value = emp.gstNumber || "";
    document.querySelector("[name='RocNumber']").value = emp.rocNumber || "";
    document.querySelector("[name='UanNumber']").value = emp.uanNumber || "";
    document.querySelector("[name='BankName']").value = emp.bankName || "";
    document.querySelector("[name='AccountNumber']").value = emp.accountNumber || "";
    document.querySelector("[name='IFSCCode']").value = emp.ifscCode || "";
    document.querySelector("[name='AccountHolderName']").value = emp.accountHolderName || "";
    //document.querySelector("[name='agreementFile']").value = emp.agreementFile || "";

    //document.querySelector("[name='signatureFile']").value = emp.signatureFile || "";

    setExistingAgreementFile(emp.agreementPdfPath);

    setExistingSignatureFile(emp.digitalSignaturePath);


    //document.getElementById("existingSignature").innerHTML =
    //    emp.signatureFile
    //        ? `<a href="${emp.signatureFile}" target="_blank">View Signature</a>`
    //        : "No file uploaded";

    vendorModal = new bootstrap.Modal(
        document.getElementById("vendorModal")
    );
    vendorModal.show();
    setTimeout(initSignaturePad, 300);

};

function setExistingAgreementFile(filePath) {
    const display = document.getElementById("agreementFileDisplay");
    if (!display) return;

    if (filePath) {
        const fileName = filePath.split('/').pop();
        display.innerHTML = `
            ${fileName}
            <a href="${filePath}" target="_blank"
               class="ms-2 text-primary text-decoration-none">(View)</a>`;
    } else {
        display.innerText = "No file chosen";
    }
}

function setExistingSignatureFile(filePath) {
    const display = document.getElementById("signatureFileDisplay");
    if (!display) return;

    if (filePath) {
        const fileName = filePath.split('/').pop();
        display.innerHTML = `
            ${fileName}
            <a href="${filePath}" target="_blank"
               class="ms-2 text-primary text-decoration-none">(View)</a>`;
    } else {
        display.innerText = "No file chosen";
    }
}

function onAgreementFileChange(input) {
    const display = document.getElementById("agreementFileDisplay");
    if (!display) return;

    if (input.files && input.files.length > 0) {
        display.innerText = input.files[0].name;
    } else {
        display.innerText = "No file chosen";
    }
}

function onSignatureFileChange(input) {
    const display = document.getElementById("signatureFileDisplay");
    if (!display) return;

    if (input.files && input.files.length > 0) {
        display.innerText = input.files[0].name;

        // ❗ Clear canvas signature if file selected
        clearPad();
        document.getElementById("signatureData").value = "";
    } else {
        display.innerText = "No file chosen";
    }
}


// LOAD EMPLOYEES
//function loadVendors() {
//    fetch(`${apiurl}/api/vendor/get-all`)
//        .then(res => res.json())
//        .then(result => {

//            if (!result.success) {
//                alert("Failed to load vendors");
//                return;
//            }

//            let rows = "";

//            result.data.forEach(ven => {
//                rows += `
//                <tr>
//                    <td>${ven.companyName}</td>
//                    <td>${ven.companyEmail ?? ""}</td>
//                    <td>${ven.companyPhone ?? ""}</td>
//                    <td>${ven.pocPhone ?? ""}</td>
//                    <td style="width: 145px;">
//                       <button class="btn btn-sm btn-outline-info action-btn"
//                                onclick="openViewModal(${ven.vendorId})">
//                            <i class="bi bi-eye"></i>
//                        </button>

//                        <button class="btn btn-sm btn-outline-primary action-btn"
//                                onclick='openEditModal(${JSON.stringify(ven)})'>
//                            <i class="bi bi-pencil-square"></i>
//                        </button>

//                        <button class="btn btn-sm btn-outline-danger action-btn"
//                                onclick="openDeleteModal(${ven.vendorId})">
//                            <i class="bi bi-trash"></i>
//                        </button>

//                    </td>
//                </tr>`;
//            });

//            document.getElementById("vendorTableBody").innerHTML = rows;

//            //  Destroy old DataTable before re-init
//            if ($.fn.DataTable.isDataTable('#vendorTable')) {
//                $('#vendorTable').DataTable().destroy();
//            }

//            //  Initialize DataTable
//            vendorTable = $('#vendorTable').DataTable({
//                pageLength: 5,
//                lengthMenu: [5, 10, 25, 50],
//                searching: true,
//                ordering: true,
//                responsive: true,
//                columnDefs: [
//                    { orderable: false, targets: 4 } // Disable sorting on Action column
//                ]
//            });
//        })
//        .catch(err => console.error(err));
//}

// FORM SUBMIT (ADD / EDIT)


function loadVendors() {

    fetch(`${apiurl}/api/vendor/get-all`)
        .then(res => res.json())
        .then(result => {

            if (!result.success) {
                alert("Failed to load vendors");
                return;
            }

            allVendors = result.data;
            filteredVendors = [...allVendors];
            currentPage = 1;

            renderVendorCards();
            updatePagination();
        })
        .catch(err => console.error(err));
}


function renderVendorCards() {

    const container = document.getElementById("vendorCardContainer");
    container.innerHTML = "";

    const start = (currentPage - 1) * pageSize;
    const end = start + pageSize;
    const pageItems = filteredVendors.slice(start, end);

    pageItems.forEach(ven => {

        const initials = ven.companyName
            .split(" ")
            .map(x => x[0])
            .join("")
            .substring(0, 2)
            .toUpperCase();

        container.innerHTML += `
        <div class="col-12 col-md-6 col-lg-4">

            <div class="vendor-card">

                <!-- ACTION BUTTONS -->
                <div class="vendor-actions">
                    <i class="bi bi-eye text-info"
                       onclick="openViewModal(${ven.vendorId})"></i>

                    <i class="bi bi-pencil-square text-primary"
                       onclick='openEditModal(${JSON.stringify(ven)})'></i>

                    <i class="bi bi-trash text-danger"
                       onclick="openDeleteModal(${ven.vendorId})"></i>
                </div>

                <!-- HEADER -->
                <div class="d-flex gap-3">
                     <!-- <div class="vendor-avatar">${initials}</div>-->
                    <div>
                        <h5 class="mb-0">${ven.companyName}</h5>
                        <small class="text-muted">
                            ${ven.companyEmail || "—"}
                        </small>
                    </div>
                </div>

                <span class="badge-active mt-2 d-inline-block">Active</span>

                <hr/>

                <div class="small mb-1">
                    <i class="bi bi-telephone"></i>
                    ${ven.companyPhone || "—"}
                </div>

                <div class="small">
                    <i class="bi bi-receipt"></i>
                    GST: ${ven.gstNumber || "—"}
                </div>

            </div>
        </div>`;
    });
}

function updatePagination() {

    const totalPages =
        Math.ceil(filteredVendors.length / pageSize);

    document.getElementById("pageInfo").innerText =
        `Page ${currentPage} of ${totalPages || 1}`;

    document.getElementById("prevBtn").disabled =
        currentPage === 1;

    document.getElementById("nextBtn").disabled =
        currentPage === totalPages || totalPages === 0;
}

document.getElementById("prevBtn").onclick = () => {
    if (currentPage > 1) {
        currentPage--;
        renderVendorCards();
        updatePagination();
    }
};

document.getElementById("nextBtn").onclick = () => {
    const totalPages =
        Math.ceil(filteredVendors.length / pageSize);

    if (currentPage < totalPages) {
        currentPage++;
        renderVendorCards();
        updatePagination();
    }
};

document.getElementById("pageSize")
    .addEventListener("change", function () {

        pageSize = parseInt(this.value);
        currentPage = 1;

        renderVendorCards();
        updatePagination();
    });

document.getElementById("searchInput")
    .addEventListener("input", function () {

        const q = this.value.toLowerCase().trim();

        filteredVendors = allVendors.filter(v =>
            `${v.companyName} ${v.companyEmail} ${v.companyPhone} ${v.gstNumber}`
                .toLowerCase()
                .includes(q)
        );

        currentPage = 1;
        renderVendorCards();
        updatePagination();
    });



document.addEventListener("DOMContentLoaded", function () {

    loadVendors();

    document.getElementById("vendorForm").addEventListener("submit", function (e) {
        e.preventDefault();

        const form = document.getElementById("vendorForm");
        const formData = new FormData(form);

        fetch(`${apiurl}/api/vendor/insert-update`, {
            method: "POST",
            body: formData
        })
            .then(async response => {

                // ❌ API error (400 / 500)
                if (!response.ok) {
                    const error = await response.json();
                    alert(error.message || error.title || "Something went wrong");
                    return;
                }

                // ✅ API success
                const res = await response.json();
                alert(res.message);

                bootstrap.Modal.getInstance(
                    document.getElementById("vendorModal")
                ).hide();
                window.location.reload();
                loadVendors();
            })
            .catch(err => {
                console.error(err);
                alert("Network error");
            });
    });

    let deleteModal;

    // OPEN DELETE MODAL
    window.openDeleteModal = function (id) {
        document.getElementById("deleteVendorId").value = id;

        deleteModal = new bootstrap.Modal(
            document.getElementById("deleteModal")
        );
        deleteModal.show();
    };

    // CONFIRM DELETE
    window.confirmDelete = function () {
        const id = document.getElementById("deleteVendorId").value;

        fetch(`${apiurl}/api/vendor/${id}`, {
            method: "DELETE"
        })
            .then(res => res.json())
            .then(result => {
                if (!result.success) {
                    alert(result.message || "Delete failed");
                    return;
                }

                alert(result.message);
                deleteModal.hide();
                loadVendors();
            })
            .catch(err => {
                console.error(err);
                alert("Network error");
            });
    };

    window.openViewModal = function (vendorId) {

        fetch(`${apiurl}/api/vendor/${vendorId}`)
            .then(res => {
                if (!res.ok) throw new Error("Vendor not found");
                return res.json();
            })
            .then(result => {

                const emp = result.data;

                let html = `
                <tr><th>Company Name</th><td>${emp.companyName}</td></tr>
                <tr><th>Company Email</th><td>${emp.companyEmail ?? ""}</td></tr>
                <tr><th>Company Phone</th><td>${emp.companyPhone ?? ""}</td></tr>
                <tr><th>Company Address</th><td>${emp.companyAddress ?? ""}</td></tr>
                <tr><th>Poc (Point of contact person)</th><td>${emp.pocPhone ?? ""}</td></tr>
                <tr><th>Gst Number</th><td>${emp.gstNumber ?? ""}</td></tr>
                <tr><th>Roc Number</th><td>${emp.rocNumber ?? ""}</td></tr>
                <tr><th>UAN Number</th><td>${emp.uanNumber ?? ""}</td></tr>
                <tr><th>Bank Name</th><td>${emp.bankName ?? ""}</td></tr>
                <tr><th>Account Number</th><td>${emp.accountNumber ?? ""}</td></tr>

                 <tr><th>IFSC Code</th><td>${emp.ifscCode ?? ""}</td></tr>
                <tr><th>Account Holder Name</th><td>${emp.accountHolderName ?? ""}</td></tr>
                <tr><th>Bank Name</th><td>${emp.bankName ?? ""}</td></tr>
                <tr><th>Account Number</th><td>${emp.accountNumber ?? ""}</td></tr>

                <tr>
                    <th>Agreement</th>
                    <td>${fileLink(emp.agreementPdfPath)}</td>
                </tr>
                <tr>
                    <th>signature</th>
                    <td>${fileLink(emp.digitalSignaturePath)}</td>
                </tr>
                
            `;

                document.getElementById("viewVendorBody").innerHTML = html;

                new bootstrap.Modal(
                    document.getElementById("viewVendorModal")
                ).show();
            })
            .catch(err => alert(err.message));
    };

    function fileLink(path) {
        if (!path)
            return "<span class='text-muted'>Not uploaded</span>";

        return `
        <a href="${path}" target="_blank" class="btn btn-sm btn-primary">
            View
        </a>
        <a href="${path}" download class="btn btn-sm btn-success ms-2">
            Download
        </a>
    `;
    }
    
});


let canvas = null;
let ctx = null;
let drawing = false;

function initSignaturePad() {
    canvas = document.getElementById("signaturePad");
    if (!canvas) return;

    const parentWidth = canvas.parentElement.offsetWidth;

    //  SET REAL CANVAS SIZE (CRITICAL)
    canvas.width = parentWidth;
    canvas.height = 200;

    ctx = canvas.getContext("2d");
    ctx.strokeStyle = "#000";
    ctx.lineWidth = 2;
    ctx.lineCap = "round";

    drawing = false;

    // REMOVE old listeners
    canvas.replaceWith(canvas.cloneNode(true));
    canvas = document.getElementById("signaturePad");
    ctx = canvas.getContext("2d");

    // MOUSE EVENTS
    canvas.addEventListener("mousedown", startDraw);
    canvas.addEventListener("mousemove", draw);
    canvas.addEventListener("mouseup", stopDraw);
    canvas.addEventListener("mouseleave", stopDraw);

    // TOUCH EVENTS
    canvas.addEventListener("touchstart", startDraw, { passive: false });
    canvas.addEventListener("touchmove", draw, { passive: false });
    canvas.addEventListener("touchend", stopDraw);

    clearPad();
}

function startDraw(e) {
    e.preventDefault();
    drawing = true;

    const pos = getPosition(e);
    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function draw(e) {
    if (!drawing) return;
    e.preventDefault();

    const pos = getPosition(e);
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}


function stopDraw(e) {
    if (!drawing) return;
    drawing = false;

    const dataUrl = canvas.toDataURL("image/png");

    // ❗ Save base64
    document.getElementById("signatureData").value = dataUrl;

    // ❗ Clear file input if drawing used
    const fileInput = document.getElementById("signatureFileInput");
    fileInput.value = "";

    document.getElementById("signatureFileDisplay").innerText =
        "Digital signature captured";

    console.log("Signature saved ✔");
}

function getPosition(e) {
    const rect = canvas.getBoundingClientRect();

    let clientX, clientY;

    if (e.touches && e.touches.length > 0) {
        clientX = e.touches[0].clientX;
        clientY = e.touches[0].clientY;
    } else {
        clientX = e.clientX;
        clientY = e.clientY;
    }

    return {
        x: clientX - rect.left,
        y: clientY - rect.top
    };
}

function clearPad() {
    if (!ctx || !canvas) return;

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    document.getElementById("signatureData").value = "";
}
