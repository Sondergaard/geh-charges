locals {
    # The string value is the shared keyvault key name
    INTEGRATION_EVENTS_LISTENER_CONNECTION_STRING       = "SHARED-RESOURCES--SB-INTEGRATIONEVENTS-LISTEN-CONNECTION-STRING"
    INTEGRATION_EVENTS_SENDER_CONNECTION_STRING         = "SHARED-RESOURCES--SB-INTEGRATIONEVENTS-SEND-CONNECTION-STRING"
    INTEGRATION_EVENTS_MANAGER_CONNECTION_STRING        = "SHARED-RESOURCES--SB-INTEGRATIONEVENTS-MANAGE-CONNECTION-STRING"

    # Message Hub
    MESSAGEHUB_STORAGE_CONNECTION_STRING_KEY            = "SHARED-RESOURCES-MARKETOPERATOR-RESPONSE-CONNECTION-STRING"
    MESSAGEHUB_STORAGE_CONTAINER_KEY                    = "SHARED-RESOURCES-MARKETOPERATOR-CONTAINER-REPLY-NAME"
}