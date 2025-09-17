resource "fusionauth_application" "forum" {
	tenant_id = fusionauth_tenant.forum.id
	name      = "forum"
	jwt_configuration {
		access_token_id = fusionauth_key.forum-access-token.id
	}
}

resource "fusionauth_application_role" "forum_admin_role" {
	application_id = fusionauth_application.forum.id
	is_default     = false
	is_super_role  = true
	name           = "admin"
}

resource "fusionauth_application_role" "forum_user_role" {
	application_id = fusionauth_application.forum.id
	is_default     = true
	is_super_role  = false
	name           = "user"
}
