resource "fusionauth_application" "resume-builder" {
  tenant_id = fusionauth_tenant.resume-builder.id
  name      = local.friendly_name
  jwt_configuration {
    access_token_id = fusionauth_key.resume-builder-access-token.id
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
