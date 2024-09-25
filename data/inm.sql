-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 25-09-2024 a las 17:02:29
-- Versión del servidor: 10.4.28-MariaDB
-- Versión de PHP: 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inm`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contrato`
--

CREATE TABLE `contrato` (
  `ID_contrato` int(11) NOT NULL,
  `ID_inmueble` int(11) NOT NULL,
  `ID_inquilino` int(11) NOT NULL,
  `Fecha_Inicio` date NOT NULL,
  `Fecha_Fin` date NOT NULL,
  `Monto_Mensual` decimal(10,2) NOT NULL,
  `Estado` tinyint(1) NOT NULL,
  `Fecha_Terminacion_Anticipada` date DEFAULT NULL,
  `Multa` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contrato`
--

INSERT INTO `contrato` (`ID_contrato`, `ID_inmueble`, `ID_inquilino`, `Fecha_Inicio`, `Fecha_Fin`, `Monto_Mensual`, `Estado`, `Fecha_Terminacion_Anticipada`, `Multa`) VALUES
(1, 1, 1, '2024-09-12', '2024-12-13', 2300.00, 1, NULL, NULL),
(2, 2, 1, '2024-09-18', '2024-11-22', 2300.00, 0, NULL, NULL),
(3, 2, 2, '2024-09-26', '2024-12-18', 2500.00, 0, NULL, NULL),
(4, 3, 2, '2024-09-28', '2024-10-29', 2800.00, 0, NULL, NULL),
(5, 2, 1, '2024-09-26', '2024-11-16', 2300.00, 0, NULL, NULL),
(6, 6, 1, '2024-09-26', '2024-10-17', 4300.00, 0, NULL, NULL),
(7, 13, 3, '2024-09-26', '2024-10-26', 2300.00, 0, NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `ID_inmueble` int(11) NOT NULL,
  `ID_propietario` int(11) DEFAULT NULL,
  `Direccion` varchar(255) NOT NULL,
  `Uso` enum('Comercial','Residencial') NOT NULL,
  `Tipo` enum('Local','Deposito','Casa','Departamento','Otro') NOT NULL,
  `Cantidad_Ambientes` int(11) NOT NULL,
  `Latitud` decimal(9,6) DEFAULT NULL,
  `Longitud` decimal(9,6) DEFAULT NULL,
  `Precio` decimal(10,2) NOT NULL,
  `Estado` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`ID_inmueble`, `ID_propietario`, `Direccion`, `Uso`, `Tipo`, `Cantidad_Ambientes`, `Latitud`, `Longitud`, `Precio`, `Estado`) VALUES
(1, 1, 'Calle del Ejemplo 456', 'Comercial', 'Casa', 4, 40.123456, -3.654323, 1200.00, 0),
(2, 1, 'Av.Juan G. Funes 785', 'Residencial', 'Departamento', 4, -33.311277, -66.333202, 1102.00, 0),
(3, 1, 'Av. Santa Fe 3223, Mendoza', 'Residencial', 'Casa', 5, -34.603722, -58.385678, 3000.00, 1),
(5, 2, 'La Obra 1051', 'Comercial', 'Deposito', 5, -34.603722, -68.827200, 59000.00, 0),
(6, 3, 'Av LaFinur 132', 'Residencial', 'Departamento', 2, -34.603580, -58.385678, 59000.99, 0),
(12, 3, 'Av. Santa Fe 3223, Mendoza', 'Residencial', 'Local', 1, -34.603722, -58.385678, 59800.00, 0),
(13, 3, 'Inmueble del contrato Desactivar', 'Residencial', 'Casa', 1, -34.603722, -58.385678, 59800.00, 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `ID_inquilino` int(11) NOT NULL,
  `DNI` varchar(20) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Direccion` varchar(255) DEFAULT NULL,
  `Estado` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilino`
--

INSERT INTO `inquilino` (`ID_inquilino`, `DNI`, `Nombre`, `Apellido`, `Telefono`, `Email`, `Direccion`, `Estado`) VALUES
(1, '87654321B', 'Ana', 'García', '555-5678', 'ana.garcia@example.com', 'Avenida Siempre Viva 742', 1),
(2, '32323232', 'Lionel', 'Messi', '2664022135', 'leo10@gmail.com', 'Miami', 0),
(3, '1234567', 'Inquilino', 'Prueba', '264000034', 'prueba@gmail.com', 'calle false', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `ID_pago` int(11) NOT NULL,
  `ID_contrato` int(11) NOT NULL,
  `Numero_pago` int(11) NOT NULL,
  `Fecha_pago` date NOT NULL,
  `Importe` decimal(10,2) NOT NULL,
  `Concepto` varchar(255) DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pago`
--

INSERT INTO `pago` (`ID_pago`, `ID_contrato`, `Numero_pago`, `Fecha_pago`, `Importe`, `Concepto`, `Estado`) VALUES
(1, 1, 1, '2024-10-12', 1200.00, 'Transferencia Bancaria', 0),
(2, 3, 1, '2024-10-26', 2500.00, 'Debito Automatico', 0),
(3, 3, 2, '2024-09-24', 20000.00, 'Efectivo', 0),
(4, 4, 1, '2024-09-25', 5800.00, 'Debito', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `ID_propietario` int(11) NOT NULL,
  `DNI` varchar(20) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Direccion` varchar(255) DEFAULT NULL,
  `Estado` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`ID_propietario`, `DNI`, `Nombre`, `Apellido`, `Telefono`, `Email`, `Direccion`, `Estado`) VALUES
(1, '12345678A', 'Juan', 'Pérez', '555-1234', 'juan.perez@example.com', 'Calle Falsa 12', 1),
(2, '32323232', 'Pepito', 'Perez', '2665987002', 'a@gmail.com', 'Av LaFinur 132', 0),
(3, '32323232', 'Naruto', 'Titi', '2664033442', 'b@bb.com', 'Belgrano 755', 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `ID_usuario` int(11) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Rol` int(11) DEFAULT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  `Avatar` varchar(255) DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`ID_usuario`, `Email`, `Password`, `Rol`, `Nombre`, `Apellido`, `Estado`, `Avatar`) VALUES
(1, 'a@gmail.com', '+/RXd2ZQPXGwm9WJOAj9KXJgnduKyb5lrII8nqTub1zfg0+t', 2, 'Pepe', 'Perez', 1, '/uploads/avatars/default.jpg'),
(2, 'asdasd@gmail.com', 'gDt0x2Z1btyIFc21Fex2Su/EzKW4wj+3jE7pC2L03bwaBGqi', 1, 'Pepito', 'Segundo', 1, '/uploads/avatars/902258c0-d5da-4d29-9df6-f7a6d98c0eb1_usuario.jpg'),
(3, 'b@b.com', 'nEAWvfkb+q7YV+KV3ZvgRPbWJlz0LTecWodYiKOU9sccVzNa', 1, 'JuanEdit', 'Segu', 1, NULL),
(4, 'probando@gmail.com', 'TEgrwZI5Jv0KZGPz8NUXt76EL0unWR/Zl4zAf9Y+4ev+kuLC', 1, 'Leoo', 'Messi', 1, '/uploads/avatars/38781f46-a80a-4a4e-b7b9-a2cb29706a24_usuario.jpg'),
(5, 'empleado2@gmail.com', 'cC+JOVFLZHKbcD1amzYiNhQln86b3Ffs+HFTydKT93zMr6Uv', 2, 'Empleado', 'Esel Roll', 1, '/uploads/avatars/b140ada9-a600-410b-8364-f0683382f84f_avatar1.jpg'),
(6, 'correo@gmail.com', '+ev+BtcbFhaqpQbfMqUDTrX41NGV7Gf+Ngw8DmYDpk9p2rlR', 2, 'Alex', 'Astudillo', 1, '/uploads/avatars/default.jpg');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`ID_contrato`),
  ADD KEY `ID_inmueble` (`ID_inmueble`),
  ADD KEY `ID_inquilino` (`ID_inquilino`);

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`ID_inmueble`),
  ADD KEY `ID_propietario` (`ID_propietario`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`ID_inquilino`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`ID_pago`),
  ADD KEY `pago_ibfk_1` (`ID_contrato`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`ID_propietario`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`ID_usuario`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contrato`
--
ALTER TABLE `contrato`
  MODIFY `ID_contrato` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `ID_inmueble` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `ID_inquilino` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `ID_pago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `ID_propietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `ID_usuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`ID_inmueble`) REFERENCES `inmueble` (`ID_inmueble`),
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`ID_inquilino`) REFERENCES `inquilino` (`ID_inquilino`);

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`ID_propietario`) REFERENCES `propietario` (`ID_propietario`);

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`ID_contrato`) REFERENCES `contrato` (`ID_contrato`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
