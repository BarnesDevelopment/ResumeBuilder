resource "fusionauth_application" "resume-builder" {
  tenant_id = fusionauth_tenant.resume-builder.id
  name      = local.friendly_name

  oauth_configuration {
    client_id = "9833eaf0-8202-4cbf-b47c-c6224c742024"
    authorized_redirect_urls = [
      "https://localhost:4200/login/callback",
    ]
    generate_refresh_tokens = true

    client_authentication_policy = "NotRequired"

    scope_handling_policy = "Compatibility"
    unknown_scope_policy  = "Remove"

    proof_key_for_code_exchange_policy = "Required"

    enabled_grants = [
      "refresh_token", "authorization_code"
    ]
    provided_scope_policy {
      address {
        enabled  = false
        required = false
      }
      email {
        enabled  = true
        required = false
      }
      phone {
        enabled  = false
        required = false
      }
      profile {
        enabled  = true
        required = false
      }
    }

  }
  jwt_configuration {
    enabled                   = true
    refresh_token_ttl_minutes = 43200
    ttl_seconds               = 3600
    access_token_id           = fusionauth_key.resume-builder-signing-key.id
  }
}

resource "fusionauth_application_role" "resume-builder_admin_role" {
  application_id = fusionauth_application.resume-builder.id
  is_default     = false
  is_super_role  = true
  name           = "admin"
}

resource "fusionauth_application_role" "resume-builder_user_role" {
  application_id = fusionauth_application.resume-builder.id
  is_default     = true
  is_super_role  = false
  name           = "user"
}
