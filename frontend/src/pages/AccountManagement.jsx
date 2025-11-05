import React, { useState } from "react";

export default function AccountManagement() {
  // Username State
  const [username, setUsername] = useState("");
  const [usernameMsg, setUsernameMsg] = useState(null);
  const [usernameLoading, setUsernameLoading] = useState(false);

  // Password State
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [passwordMsg, setPasswordMsg] = useState(null);
  const [passwordLoading, setPasswordLoading] = useState(false);

  // Delete Account State 
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [deleteMsg, setDeleteMsg] = useState(null);

  // Update Username 
  async function handleUpdateUsername(e) {
    e.preventDefault();
    setUsernameMsg(null);

    if (!username || username.trim().length < 3) {
      setUsernameMsg({
        type: "error",
        text: "Username must be at least 3 characters.",
      });
      return;
    }

    setUsernameLoading(true);
    try {
      // Adjust this endpoint as needed
      const response = await fetch("", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username: username.trim() }),
      });

      if (!response.ok) {
        // eslint-disable-next-line no-undef
        const error = await response.json().catch(() => ({
          message: "Failed to update username",
        }));
        throw new Error(error.message || "Failed to update username");
      }

      setUsernameMsg({ type: "success", text: "Username updated successfully." });
    } catch (error) {
      setUsernameMsg({ type: "error", text: error.message });
    } finally {
      setUsernameLoading(false);
    }
  }

  // Update Password
  async function handleUpdatePassword(e) {
    e.preventDefault();
    setPasswordMsg(null);

    if (!newPassword || newPassword.length < 8) {
      setPasswordMsg({
        type: "error",
        text: "New password must be at least 8 characters.",
      });
      return;
    }

    if (newPassword !== confirmPassword) {
      setPasswordMsg({
        type: "error",
        text: "New password and confirmation do not match.",
      });
      return;
    }

    setPasswordLoading(true);
    try {
      // Adjust this endpoint as needed
      const response = await fetch("", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ newPassword }),
      });

      if (!response.ok) {
        // eslint-disable-next-line no-undef
        const error = await response.json().catch(() => ({
          message: "Failed to update password",
        }));
        throw new Error(error.message || "Failed to update password");
      }

      setPasswordMsg({ type: "success", text: "Password updated successfully." });
      setNewPassword("");
      setConfirmPassword("");
    } catch (error) {
      setPasswordMsg({ type: "error", text: error.message });
    } finally {
      setPasswordLoading(false);
    }
  }

  // Delete Account
  async function handleDeleteAccount() {
    setDeleteMsg(null);
    setDeleteLoading(true);
    try {
      // Adjust this endpoint as needed
      const response = await fetch("", {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      });

      if (!response.ok) {
        // eslint-disable-next-line no-undef
        const error = await response.json().catch(() => ({
          message: "Failed to delete account",
        }));
        throw new Error(error.message || "Failed to delete account");
      }

      setDeleteMsg({
        type: "success",
        text: "Account deleted. Redirecting...",
      });
      setTimeout(() => {
        // Adjust redirection as needed
        window.location.href = "/";
      }, 1200);
    } catch (error) {
      setDeleteMsg({ type: "error", text: error.message });
    } finally {
      setDeleteLoading(false);
    }
  }

  // UI
  return (
    <div style={{ maxWidth: 720, margin: "24px auto", padding: 16 }}>
      <h2>Account Management</h2>

      {/* Update Username */}
      <section style={{ border: "1px solid #ddd", padding: 16, marginBottom: 16 }}>
        <h3>Update Username</h3>
        <form onSubmit={handleUpdateUsername}>
          <div style={{ marginBottom: 8 }}>
            <label>
              New username
              <input
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                style={{ display: "block", width: "100%", padding: 8, marginTop: 4 }}
                placeholder="Enter new username"
              />
            </label>
          </div>
          <button type="submit" disabled={usernameLoading}>
            {usernameLoading ? "Saving..." : "Save username"}
          </button>
        </form>
        {usernameMsg && (
          <div
            style={{
              marginTop: 8,
              color: usernameMsg.type === "error" ? "red" : "green",
            }}
          >
            {usernameMsg.text}
          </div>
        )}
      </section> 

      {/* Update Password */}
      <section style={{ border: "1px solid #ddd", padding: 16, marginBottom: 16 }}>
        <h3>Update Password</h3>
        <form onSubmit={handleUpdatePassword}>
          <div style={{ marginBottom: 8 }}>
            <label>
              New password
              <input
                type="password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                style={{ display: "block", width: "100%", padding: 8, marginTop: 4 }}
                placeholder="New password (min 8 chars)"
              />
            </label>
          </div>

          <div style={{ marginBottom: 8 }}>
            <label>
              Confirm new password
              <input
                type="password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                style={{ display: "block", width: "100%", padding: 8, marginTop: 4 }}
                placeholder="Confirm new password"
              />
            </label>
          </div>

          <button type="submit" disabled={passwordLoading}>
            {passwordLoading ? "Saving..." : "Save password"}
          </button>
        </form>
        {passwordMsg && (
          <div
            style={{
              marginTop: 8,
              color: passwordMsg.type === "error" ? "red" : "green",
            }}
          >
            {passwordMsg.text}
          </div>
        )}
      </section>

      {/* Delete Account */}
      <section style={{ border: "1px solid #f5c6cb", padding: 16, background: "#fff5f5" }}>
        <h3 style={{ color: "#a71d2a" }}>Delete Account</h3>
        <p>This action is permanent. All your data will be removed.</p>
        <button
          onClick={handleDeleteAccount}
          disabled={deleteLoading}
          style={{
            background: "#a71d2a",
            color: "#fff",
            padding: "8px 12px",
            border: "none",
          }}
        >
          {deleteLoading ? "Deleting..." : "Delete account"}
        </button>
        {deleteMsg && (
          <div
            style={{
              marginTop: 8,
              color: deleteMsg.type === "error" ? "red" : "green",
            }}
          >
            {deleteMsg.text}
          </div>
        )}
      </section>
    </div>
  );
}
