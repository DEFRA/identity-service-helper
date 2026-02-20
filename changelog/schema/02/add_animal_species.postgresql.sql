-- liquibase formatted sql

-- changeset cedricbrasey:1771532961448-1 splitStatements:false
CREATE TABLE "animal_species" (
    "id" VARCHAR(20) NOT NULL,
    "name" VARCHAR(128) NOT NULL,
    "is_active" boolean NOT NULL DEFAULT false,
    CONSTRAINT "animal_species_pk" PRIMARY KEY ("id")
);
