resource "fusionauth_application" "resume-builder" {
  tenant_id = fusionauth_tenant.resume-builder.id
  name      = local.friendly_name
  oauth_configuration {
    enabled               = true
    client_id             = var.fusionauth_default_application_id # Optional: Specify a custom client_id
    authorized_redirect_urls = [
      "http://localhost:4200/login/callback",
    ]
    generate_refresh_tokens = true

    scope_handling_policy = "Strict"
    unknown_scope_policy  = "Reject"
    
    enabled_grants = [
      "authorization_code", "implicit"
    ]
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
