-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 12-11-2024 a las 01:43:35
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
(1, 3, 1, '2024-09-12', '2024-12-13', 220000.00, 1, NULL, NULL),
(2, 2, 1, '2024-09-18', '2024-11-22', 2300.00, 0, NULL, NULL),
(3, 2, 2, '2024-09-26', '2024-12-18', 2500.00, 0, NULL, NULL),
(4, 3, 2, '2024-09-28', '2024-10-29', 2800.00, 0, NULL, NULL),
(5, 2, 1, '2024-09-26', '2024-11-16', 2300.00, 0, NULL, NULL),
(6, 6, 3, '2024-09-26', '2024-10-17', 4300.00, 0, NULL, NULL),
(7, 38, 2, '2024-09-26', '2024-10-26', 2300.00, 1, NULL, NULL),
(8, 52, 1, '2024-09-25', '2024-11-25', 189000.00, 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `ID_inmueble` int(11) NOT NULL,
  `ID_propietario` int(11) DEFAULT NULL,
  `Direccion` varchar(255) NOT NULL,
  `Uso` int(11) DEFAULT NULL,
  `Tipo` int(11) DEFAULT NULL,
  `Cantidad_Ambientes` int(11) NOT NULL,
  `Latitud` decimal(9,6) DEFAULT NULL,
  `Longitud` decimal(9,6) DEFAULT NULL,
  `Precio` decimal(10,2) NOT NULL,
  `Estado` tinyint(1) DEFAULT NULL,
  `Disponibilidad` tinyint(1) NOT NULL DEFAULT 1,
  `Foto` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`ID_inmueble`, `ID_propietario`, `Direccion`, `Uso`, `Tipo`, `Cantidad_Ambientes`, `Latitud`, `Longitud`, `Precio`, `Estado`, `Disponibilidad`, `Foto`) VALUES
(1, 1, 'Calle del Ejemplo 456', 1, 3, 4, 40.123456, -3.654323, 1200.00, 0, 1, ''),
(2, 1, 'Av.Juan G. Funes 785', 2, 4, 4, -33.311277, -66.333202, 1102.00, 0, 1, ''),
(3, 1, 'Av. Santa Fe 3223, Mendoza', 2, 3, 5, -34.603722, -58.385678, 187000.00, 1, 1, ''),
(5, 2, 'La Obra 1051', 1, 2, 5, -34.603722, -68.827200, 59000.00, 0, 1, ''),
(6, 3, 'Av LaFinur 132', 2, 4, 2, -34.603580, -58.385678, 59000.99, 0, 1, ''),
(12, 3, 'Av. Santa Fe 3223, Mendoza', 2, 1, 1, -34.603722, -58.385678, 59800.00, 0, 1, ''),
(13, 3, 'Inmueble del contrato Desactivar', 2, 4, 1, -34.603722, -58.385678, 59800.00, 0, 0, ''),
(14, 4, 'Av LaFinur 423', 1, 1, 6, -34.603722, -58.385678, 28000.00, 1, 1, ''),
(15, 1, 'Belgrano 754', 2, 4, 1, -34.603722, -58.385678, 598000.00, 0, 1, ''),
(16, 4, 'Belgrano 755', 1, 3, 3, -34.603722, -58.385678, 59000.99, 1, 1, ''),
(17, 3, 'Av LaFinur 423', 2, 4, 4, -34.603722, -58.385678, 280000.00, 1, 1, ''),
(38, 6, 'Juana Koslay 899', 2, 3, 5, -34.000000, -58.000000, 750000.99, 1, 1, '/uploads/fotos/2621d8e8-294b-4d19-9cbb-8c657254cbd4_inmueble.png'),
(39, 7, 'San Martin 447', 1, 3, 4, -33.009999, -58.990902, 199000.99, 1, 0, '/uploads/fotos/ef4363fc-0d21-49a3-b04d-f4e49b35b8cf_loading.jpg'),
(52, 6, 'Av. Juan Gilberto Funes', 1, 1, 1, 999.999999, 999.999999, 58300033.00, 1, 0, '/uploads/fotos/732f84b1-bd84-4181-ab61-6087a3df2075_image.png');

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
(2, '32323232', 'Juan ', 'Perez', '2664022135', 'perez10@gmail.com', 'Miami', 1),
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
(2, 7, 1, '2024-09-26', 2300.00, 'Debito Automatico', 0),
(3, 7, 2, '2024-10-26', 2300.00, 'Efectivo', 0),
(4, 8, 1, '2024-09-25', 5800.00, 'Transferencia bancaria', 1);

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
  `Estado` tinyint(1) DEFAULT 1,
  `Password` varchar(255) NOT NULL,
  `Avatar` varchar(255) DEFAULT NULL,
  `ResetToken` varchar(255) DEFAULT NULL,
  `ResetTokenExpiry` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`ID_propietario`, `DNI`, `Nombre`, `Apellido`, `Telefono`, `Email`, `Direccion`, `Estado`, `Password`, `Avatar`, `ResetToken`, `ResetTokenExpiry`) VALUES
(1, '12345678A', 'Juan', 'Pérez', '555-12345', 'juan.perez@example.com', 'Calle Falsa 12', 1, '', '', NULL, NULL),
(2, '32323232', 'Pepito', 'Perez', '2665987002', 'a@gmail.com', 'Av LaFinur 132', 0, '123', '', NULL, NULL),
(3, '32323232', 'Naruto', 'Titi', '2664033442', 'b@bb.com', 'Belgrano 755', 1, '', '', NULL, NULL),
(4, '18122022', 'Leo', 'Messi', '2664101930', 'leo@gmail.com', 'Florida Miami', 1, '', '', NULL, NULL),
(5, '12345678', 'Dibu', 'Martinez', '2664121222', 'b@a.com', 'Qatar 22', 1, '123', '', NULL, NULL),
(6, '234565', 'Lionel', 'Messi', '2664231', 'astudilloalex07@gmail.com', 'Qatar22', 1, 'rV3LK/LOJ4ZGlGoZkL4yPWyT72t9cV4UOSLy1bMd6f1ICzVo', '/uploads/avatars/0155dfef-0465-4cd6-92bf-582709d3c15b_5e721e25a6e80ead3ce0ddae20d266cf.jpg', '2479997d-84b8-499e-891b-77b4523587e5', '2024-11-10 19:47:16'),
(7, '12345678', 'Juan', 'Perez', '1234567890', 'juan.perezz@example.com', 'Calle Falsa 123', 1, 'A6A2Zb/rmz41H1cYSlyT1EXZPhpyOH7DWUPI7kL7OBy4Yans', '/uploads/avatars/5f6d0178-726f-4318-a654-a24f31874daf_Screenshot_20241011-225658.png', NULL, NULL);

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
(2, 'asdasd@gmail.com', 'gDt0x2Z1btyIFc21Fex2Su/EzKW4wj+3jE7pC2L03bwaBGqi', 1, 'Pepito', 'Segundo', 1, '/uploads/avatars/default.jpg'),
(3, 'b@b.com', 'nEAWvfkb+q7YV+KV3ZvgRPbWJlz0LTecWodYiKOU9sccVzNa', 1, 'JuanEdit', 'Segun', 1, '/uploads/avatars/default.jpg'),
(4, 'probando@gmail.com', 'TEgrwZI5Jv0KZGPz8NUXt76EL0unWR/Zl4zAf9Y+4ev+kuLC', 1, 'Leoo', 'Messi', 0, '/uploads/avatars/default.jpg'),
(5, 'empleado2@gmail.com', 'cC+JOVFLZHKbcD1amzYiNhQln86b3Ffs+HFTydKT93zMr6Uv', 2, 'Empleado', 'Esel Rol', 0, '/uploads/avatars/default.jpg'),
(6, 'empleado@gmail.com', 'NFyJpbnP1Qzv/VhrMRI/1fRx+Uk9MsruQX6VLvqLtPwblN+D', 2, 'Employee', 'EmpleadoEdit9', 1, '/uploads/avatars/default.jpg'),
(7, 'admin@gmail.com', '157HKRrs0pORrrCEwTJexS8x2Xy9rRGR4ObGn9TW07IweMfB', 1, 'Admin', 'Administrador', 1, '/uploads/avatars/80c8b4e9-cc7e-4cdb-a58a-f48cb7c6e5dc_avatar1.jpg'),
(8, 'prueba@gmail.com', 'rRkqYAiAHMmsQ8aBv246qCTELhidZ5jDdjU2YVQX5ANOhtDO', 2, 'Prueba', 'Pass', 1, '/uploads/avatars/default.jpg'),
(9, 'titi@gmail.com', 'pO+U66MRk0sZ+0opn7Q+5aQrjo5mCKeyl7lLkPb1Dkx8Jj3R', 2, 'Naruto', 'Titi', 1, '/uploads/avatars/default.jpg');

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
  MODIFY `ID_contrato` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `ID_inmueble` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=54;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `ID_inquilino` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `ID_pago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `ID_propietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `ID_usuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

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
